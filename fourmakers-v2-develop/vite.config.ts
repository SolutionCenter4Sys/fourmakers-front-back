import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'

// Plugin para ignorar erros de source maps ausentes e garantir MIME types corretos
const ignoreSourceMapErrors = () => {
  return {
    name: 'ignore-sourcemap-errors',
    configureServer(server: any) {
      server.middlewares.use((req: any, res: any, next: any) => {
        // Garantir MIME type correto para arquivos JavaScript
        if (req.url?.endsWith('.js') || req.url?.includes('.js?')) {
          res.setHeader('Content-Type', 'application/javascript; charset=utf-8')
        }
        
        // Intercepta requisições de source maps de node_modules e .vite/deps
        if (
          req.url?.includes('.map') && 
          (req.url?.includes('node_modules') || req.url?.includes('.vite/deps'))
        ) {
          res.statusCode = 404
          res.end()
          return
        }
        next()
      })
    },
  }
}

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiTarget = env.VITE_API_FOURMAKERS_URL
  const isCI = process.env.CI === 'true' || process.env.GITLAB_CI === 'true'

  return {
    plugins: [react(), ignoreSourceMapErrors()],
    server: {
      port: 8080,
      strictPort: true,
      proxy: apiTarget
        ? {
            '/api': {
              target: apiTarget,
              changeOrigin: true,
              secure: true,
            },
          }
        : undefined,
      allowedHosts: [
        'localhost',
        '127.0.0.1',
        '::1',
        'kinglier-renay-nonpneumatically.ngrok-free.dev',
      ],
      sourcemapIgnoreList: (sourcePath) => {
        // Ignora source maps de node_modules e .vite/deps para evitar erros
        return sourcePath.includes('node_modules') || sourcePath.includes('.vite/deps')
      },
      // Garantir que arquivos JS sejam servidos com MIME type correto
      middlewareMode: false,
      fs: {
        strict: false,
      },
    },
    css: {
      devSourcemap: !isCI,
    },
    esbuild: {
      sourcemap: !isCI,
    },
    optimizeDeps: {
      include: [
        'firebase/app',
        'firebase/analytics',
        'firebase/auth',
        'firebase/firestore',
        'firebase/storage',
        'firebase/performance',
      ],
      esbuildOptions: {
        // Desabilita source maps para dependências otimizadas (melhora performance)
        sourcemap: false,
      },
    },
    build: {
      // Em CI: usa 'hidden' (não inclui referência no JS final, economiza memória)
      // Local: usa true (facilita debug)
      sourcemap: isCI ? 'hidden' : true,
      assetsDir: 'assets',
      // Otimizações para CI
      ...(isCI && {
        minify: 'esbuild', // esbuild é mais rápido que terser
        chunkSizeWarningLimit: 2000, // Evita warnings desnecessários
      }),
      rollupOptions: {
        output: {
          // Garante que os arquivos tenham extensões corretas para MIME types
          assetFileNames: 'assets/[name].[ext]',
          chunkFileNames: 'assets/[name]-[hash].js',
          entryFileNames: 'assets/[name]-[hash].js',
        },
        onwarn(warning, warn) {
          // Ignora avisos sobre source maps ausentes
          if (warning.code === 'SOURCEMAP_ERROR' || warning.message?.includes('source map')) {
            return
          }
          warn(warning)
        },
      },
    },
    resolve: {
      alias: [
        { find: /^@\/lib\/utils$/, replacement: path.resolve(__dirname, 'src/shared/utils/cn.ts') },
        { find: /^@\/hooks\/(.+)$/, replacement: path.resolve(__dirname, 'src/presentation/hooks/$1') },
        { find: '@app', replacement: path.resolve(__dirname, 'src/app') },
        { find: '@core', replacement: path.resolve(__dirname, 'src/core') },
        { find: '@data', replacement: path.resolve(__dirname, 'src/data') },
        { find: '@domain', replacement: path.resolve(__dirname, 'src/domain') },
        { find: '@presentation', replacement: path.resolve(__dirname, 'src/presentation') },
        { find: '@styles', replacement: path.resolve(__dirname, 'src/presentation/styles') },
        { find: '@shared', replacement: path.resolve(__dirname, 'src/shared') },
        { find: '@assets', replacement: path.resolve(__dirname, 'src/assets') },
        { find: '@', replacement: path.resolve(__dirname, './src') },
      ],
    },
  }
})
