import { initializeApp, getApps } from 'firebase/app'
import type { FirebaseApp } from 'firebase/app'
import { getAuth } from 'firebase/auth'
import type { Auth } from 'firebase/auth'
import { getFirestore } from 'firebase/firestore'
import type { Firestore } from 'firebase/firestore'
import { getStorage } from 'firebase/storage'
import type { FirebaseStorage } from 'firebase/storage'
import { getPerformance } from 'firebase/performance'
import type { FirebasePerformance } from 'firebase/performance'
import { getAnalytics } from 'firebase/analytics'
import type { Analytics } from 'firebase/analytics'

// Validação das variáveis de ambiente obrigatórias
const requiredEnvVars = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
}

// Verifica se todas as variáveis obrigatórias estão definidas
const missingVars = Object.entries(requiredEnvVars)
  .filter(([_, value]) => !value)
  .map(([key]) => key)

if (missingVars.length > 0) {
  console.warn(
    `[Firebase] Variáveis de ambiente ausentes: ${missingVars.join(', ')}. ` +
    'Certifique-se de configurar todas as variáveis no arquivo .env'
  )
}

// Configuração do Firebase a partir das variáveis de ambiente
const firebaseConfig = {
  apiKey: requiredEnvVars.apiKey || '',
  authDomain: requiredEnvVars.authDomain || '',
  projectId: requiredEnvVars.projectId || '',
  storageBucket: requiredEnvVars.storageBucket || '',
  messagingSenderId: requiredEnvVars.messagingSenderId || '',
  appId: requiredEnvVars.appId || '',
  measurementId: import.meta.env.VITE_FIREBASE_MEASUREMENT_ID,
}

// Inicializa o Firebase apenas se ainda não foi inicializado
let app: FirebaseApp | undefined
try {
  if (getApps().length === 0) {
    try {
      app = initializeApp(firebaseConfig)
      console.log('[Firebase] Inicializado com sucesso')
    } catch (error) {
      console.error('[Firebase] Erro ao inicializar:', error)
      // Não lança o erro para evitar quebrar o módulo
      // O app ficará undefined e os serviços serão null
    }
  } else {
    app = getApps()[0]
  }
} catch (error) {
  console.error('[Firebase] Erro ao verificar apps existentes:', error)
  app = undefined
}

// Inicializa os serviços do Firebase com tratamento de erro
let authInstance: Auth | null = null
let dbInstance: Firestore | null = null
let storageInstance: FirebaseStorage | null = null

if (app) {
  try {
    authInstance = getAuth(app)
  } catch (error) {
    console.warn('[Firebase] Erro ao inicializar Auth:', error)
  }

  try {
    dbInstance = getFirestore(app)
  } catch (error) {
    console.warn('[Firebase] Erro ao inicializar Firestore:', error)
  }

  try {
    storageInstance = getStorage(app)
  } catch (error) {
    console.warn('[Firebase] Erro ao inicializar Storage:', error)
  }
}

// Exporta os serviços
// Nota: Em runtime, esses valores podem ser null se houver erro na inicialização
// Usamos type assertion para manter compatibilidade com código existente
// O código que usa esses serviços deve verificar se estão disponíveis antes de usar
export const auth = authInstance as Auth
export const db = dbInstance as Firestore
export const storage = storageInstance as FirebaseStorage

// Inicializa o Performance Monitoring (apenas no browser)
let performance: FirebasePerformance | null = null
if (typeof window !== 'undefined' && app) {
  try {
    performance = getPerformance(app)
    // Desabilita traces automáticos (OOB) que podem enviar valores inválidos
    // (ex.: className de elemento > 100 chars) e causar FirebaseError.
    performance.instrumentationEnabled = false
    console.log('[Firebase Performance] Inicializado com sucesso')
  } catch (error) {
    console.warn('[Firebase Performance] Erro ao inicializar:', error)
  }
}

// Inicializa o Analytics (apenas no browser e se o measurementId estiver configurado)
let analytics: Analytics | null = null
if (typeof window !== 'undefined' && app && firebaseConfig.measurementId) {
  try {
    analytics = getAnalytics(app)
    console.log('[Firebase Analytics] Inicializado com sucesso')
  } catch (error) {
    console.warn('[Firebase Analytics] Erro ao inicializar:', error)
  }
}

export { performance, analytics }

export default app

