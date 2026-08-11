export const LOG = {
  banner: (msg: string) => {
    const linha = '═'.repeat(54)
    console.log(`\n╔${linha}╗`)
    console.log(`║  ${msg.padEnd(53)}║`)
    console.log(`╚${linha}╝`)
  },
  secao: (codigo: string, tipo: string, titulo: string) => {
    console.log(`\n┌─────────────────────────────────────────────────────`)
    console.log(`│ ${codigo} · [${tipo}]  ${titulo}`)
    console.log(`└─────────────────────────────────────────────────────`)
  },
  passo: (msg: string) => console.log(`   → ${msg}`),
  ok: (msg: string) => console.log(`   ✅ ${msg}`),
  alerta: (msg: string) => console.log(`   ⚠️  ${msg}`),
  tempo: (inicio: number) => {
    const s = ((Date.now() - inicio) / 1000).toFixed(1)
    console.log(`   ⏱  Tempo total: ${s}s`)
  },
}
