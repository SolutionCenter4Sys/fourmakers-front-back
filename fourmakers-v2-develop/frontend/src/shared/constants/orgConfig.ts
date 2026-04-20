export interface OrgLoginConfig {
  slugRota: string
  rotaAntiga?: string
  orgId: number
  possuiSSO: boolean
  urlSSoDev: string | null
  urlSSoHomolog: string | null
  urlSSoProd: string | null
  exibeFormularioEmail: boolean
  nomeEmpresa: string | null
  rotaInicialAposSSO?: string
  rotaInicialAposSSOFlutterflow?: boolean
}

export const orgLoginConfigs: OrgLoginConfig[] = [
  {
    slugRota: 'default',
    orgId: 0,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: null,
  },
  {
    slugRota: 'foursys', // por default o sistema sempre vai papssar /login/slugRota
    orgId: 2,
    possuiSSO: true,
    urlSSoDev:
      'https://login.microsoftonline.com/8f0133fa-8efb-40b1-8ac6-37c78469f445/oauth2/v2.0/authorize?response_type=code&client_id=fe473897-36a6-4d42-b992-6fe50af4b310&scope=openid%20profile%20offline_access&redirect_uri=http://localhost:8080/sso&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    urlSSoHomolog:
      'https://login.microsoftonline.com/8f0133fa-8efb-40b1-8ac6-37c78469f445/oauth2/v2.0/authorize?response_type=code&client_id=fe473897-36a6-4d42-b992-6fe50af4b310&scope=openid%20profile%20offline_access&redirect_uri=https://app.dev.fourmakers.io/sso&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    urlSSoProd:
      'https://login.microsoftonline.com/8f0133fa-8efb-40b1-8ac6-37c78469f445/oauth2/v2.0/authorize?response_type=code&client_id=fe473897-36a6-4d42-b992-6fe50af4b310&scope=openid%20profile%20offline_access&redirect_uri=https://app.fourmakers.io/sso&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    exibeFormularioEmail: false,
    nomeEmpresa: 'Foursys',
    rotaAntiga: 'foursys',
    rotaInicialAposSSO: '/dashboard',
    rotaInicialAposSSOFlutterflow: false,
  },
  {
    slugRota: 'numen',
    orgId: 4,
    possuiSSO: true,
    urlSSoDev: 'https://login.microsoftonline.com/16ea7313-14fd-46a6-bece-761aa059c6f1/oauth2/v2.0/authorize?response_type=code&client_id=c104a924-9d83-4b68-ae91-3cb7323eb44b&scope=openid%20profile%20offline_access&redirect_uri=https://wf.dev.fourmakers.io/login2&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain',
    urlSSoHomolog: 'https://login.microsoftonline.com/16ea7313-14fd-46a6-bece-761aa059c6f1/oauth2/v2.0/authorize?response_type=code&client_id=3d989485-a627-46b4-b4d4-756de8f8d21b&scope=openid%20profile%20offline_access&redirect_uri=https://app.dev.fourmakers.io/login2&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain',
    urlSSoProd: 'https://login.microsoftonline.com/16ea7313-14fd-46a6-bece-761aa059c6f1/oauth2/v2.0/authorize?response_type=code&client_id=9016f124-cbd4-410a-84a3-9caffa87bfbc&scope=openid%20profile%20offline_access&redirect_uri=https://app.fourmakers.io/login2&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain',
    exibeFormularioEmail: false,
    nomeEmpresa: 'Numen IT',
    rotaAntiga: 'numen',
    rotaInicialAposSSO: '/dashboard',
    rotaInicialAposSSOFlutterflow: false,
  },
  {
    slugRota: 'showcase',
    orgId: 5,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'Showcase',
    rotaAntiga: 'showcase',
  },
  {
    slugRota: 'novacoop/2882',
    orgId: 6,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'NovaCoop',
    rotaAntiga: 'novacoop/2882',
  },
  {
    slugRota: 'fmu',
    orgId: 7,
    possuiSSO: true,
    urlSSoDev: 'https://login.microsoftonline.com/d55f6b89-e844-4c2b-b069-ef293ba546ff/oauth2/v2.0/authorize?response_type=code&client_id=0772e01c-09f2-45b1-ab97-a97bc3170c5a&scope=openid%20profile%20offline_access&redirect_uri=https://wf.dev.fourmakers.io/loginfmu&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    urlSSoHomolog: 'https://login.microsoftonline.com/d55f6b89-e844-4c2b-b069-ef293ba546ff/oauth2/v2.0/authorize?response_type=code&client_id=06ec3bdd-6c7c-4b73-9957-40073e205a94&scope=openid%20profile%20offline_access&redirect_uri=https://app.dev.fourmakers.io/loginfmu&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    urlSSoProd: 'https://login.microsoftonline.com/d55f6b89-e844-4c2b-b069-ef293ba546ff/oauth2/v2.0/authorize?response_type=code&client_id=a5b3cccf-6786-4883-bc70-bb9349ec4bac&scope=openid%20profile%20offline_access&redirect_uri=https://app.fourmakers.io/loginfmu&code_challenge=47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU&code_challenge_method=plain&linktarget=_self',
    exibeFormularioEmail: false,
    nomeEmpresa: 'FMU',
    rotaAntiga: 'fmu/login',
    rotaInicialAposSSO: '/dashboard',
    rotaInicialAposSSOFlutterflow: false,
  },
  {
    slugRota: 'royal',
    orgId: 9,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'Royal',
    rotaAntiga: 'royal',
  },
  {
    slugRota: 'tsbox',
    orgId: 9,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'Tsbox',
    rotaAntiga: 'tsbox',
  },
  {
    slugRota: 'trial',
    orgId: 8,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'Trial',
    rotaAntiga: 'trial',
  },
  {
    slugRota: 'ebv',
    orgId: 10,
    possuiSSO: false,
    urlSSoDev: null,
    urlSSoHomolog: null,
    urlSSoProd: null,
    exibeFormularioEmail: true,
    nomeEmpresa: 'EBV',
    rotaAntiga: 'ebv',
  },
]

export const getRedirectPathAposSSO = (orgId: number): string => {
  const orgConfig = orgLoginConfigs.find((config) => config.orgId === orgId)
  const configuredPath = orgConfig?.rotaInicialAposSSO

  if (!configuredPath) return '/dashboard'

  if (orgConfig?.rotaInicialAposSSOFlutterflow) {
    const flutterflowPath = configuredPath.startsWith('/') ? configuredPath.slice(1) : configuredPath
    return `/page/${flutterflowPath}`
  }

  return configuredPath
}

const normalizeToAbsolutePath = (path: string): string => {
  const trimmed = path.trim()
  if (!trimmed) return '/'
  return trimmed.startsWith('/') ? trimmed : `/${trimmed}`
}

export const getOrgIdFromLegacySsoRedirectPath = (pathname: string): number | null => {
  const normalizedPathname = normalizeToAbsolutePath(pathname)

  const matchedOrg = orgLoginConfigs.find((config) => {
    const configured = config.rotaInicialAposSSO
    if (!configured) return false
    return normalizeToAbsolutePath(configured) === normalizedPathname
  })

  return matchedOrg?.orgId ?? null
}
