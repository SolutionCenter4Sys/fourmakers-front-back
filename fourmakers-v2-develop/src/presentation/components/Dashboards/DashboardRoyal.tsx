import { useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { useNavigate } from 'react-router-dom'
import { Card, CardContent } from '@/components/ui/card'
import { HeaderBanner } from './HeaderBanner'
import { ShortcutCard } from './ShortcutCard'
import { AniversariantesCard } from './AniversariantesCard'
import { ActionCard } from './ActionCard'
import { CanalDenuncias } from './CanalDenuncias'
import { PesquisaSatisfacaoModal } from '@presentation/components/dashboard'
import { useParametros } from '@presentation/hooks/useParametros'
import { User, TrendingUp, ArrowUpDown } from '@/components/ui/system-icons'
import foursysHumanImage from '@/assets/home_assets/royal/foursys_human.png'
import gradientBgImage from '@/assets/home_assets/royal/gradient_bg.jpg'

interface ShortcutConfig {
  icon: React.ReactNode
  label: string
  path: string
}

export const DashboardRoyal = () => {
  const { user } = useAppSelector((state) => state.auth)
  const navigate = useNavigate()
  const [isPesquisaModalOpen, setIsPesquisaModalOpen] = useState(false)
  const { getParametro } = useParametros()
  
  // Verifica se o parâmetro CANAL_DENUNCIA_CONFIGURACOES existe
  const hasCanalDenunciaConfig = getParametro('CANAL_DENUNCIA_CONFIGURACOES') !== null
  console.log(hasCanalDenunciaConfig)
  const handleNavigate = (path: string) => {
    navigate(path)
  }

  // Configuração de shortcuts usando objetos
  const shortcutsConfig: ShortcutConfig[] = [
    {
      icon: <User className="h-6 w-6 text-primary" />,
      label: 'Dados pessoais',
      path: '/page/dadospessoais',
    },
    {
      icon: <TrendingUp className="h-6 w-6 text-primary" />,
      label: 'Perfil 360',
      path: '/page/cvdigital',
    },
    {
      icon: <ArrowUpDown className="h-6 w-6 text-primary" />,
      label: 'Reembolso',
      path: '/reembolso',
    },
  ]

  return (
    <div className="flex-1 min-h-screen ">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-6 lg:py-8 max-w-7xl">
        {/* 1️⃣ HEADER - Card grande com gradiente e imagem */}
        <HeaderBanner />

        {/* 2️⃣ GRID PRINCIPAL - 5 colunas (desktop): Aniversariantes 2, Direita 3 */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-6 mb-6 lg:items-stretch">
          {/* COLUNA ESQUERDA - Card Aniversariantes (2 colunas) */}
          <div className="lg:col-span-2 lg:h-full max-h-[445px] overflow-hidden">
              <AniversariantesCard />
          </div>

          {/* COLUNA DIREITA (3 colunas) */}
          <div className="lg:col-span-3 lg:h-full flex flex-col space-y-6">
            {/* Linha superior - Cards de atalho (3 cards horizontais) */}
            <div className="grid grid-cols-3 gap-4">
              {shortcutsConfig.map((shortcut, index) => (
                <ShortcutCard
                  key={index}
                  icon={shortcut.icon}
                  label={shortcut.label}
                  onClick={() => handleNavigate(shortcut.path)}
                />
              ))}
            </div>

            {/* Mensagem de boas-vindas - texto simples (não em card) */}
            <div className="text-left">
              <h2 className="text-2xl font-bold mb-1">
                Olá, {user?.nomeColaborador || 'Usuário'}!
              </h2>
              <p className="text-base ">
                Seja bem-vindo ao Fourmakers
              </p>
            </div>

            {/* Card "Ficou com dúvidas?" com gradiente roxo/verde e ilustração */}
            <Card
              className="border-0 overflow-hidden rounded-lgToken shadow-softToken relative"
              style={{
                backgroundImage: `url(${gradientBgImage})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
              }}
            >
              <CardContent className="p-10 relative z-10 min-h-[240px]">
                <div className="flex-1 text-white pr-0 md:pr-60">
                  <h3 className="text-2xl font-bold mb-6">
                    Ficou com dúvidas?
                  </h3>
                  <p className="text-xl text-white/90">
                    Consulte nosso manual e encontre as respostas que precisa.
                  </p>
                </div>
                {/* Imagem posicionada absolutamente na direita, sem padding */}
                <div className="hidden md:block absolute bottom-0 right-0">
                  <img
                    src={foursysHumanImage}
                    alt="Avatar corporativo"
                    className="h-60 w-auto object-contain"
                  />
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        {/* 3️⃣ SEÇÃO INFERIOR - 2 cards lado a lado */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
          {/* Card 1 – Atualização de currículo */}
          <ActionCard
            title="Está preparado para a próxima grande oportunidade?"
            buttonText="Atualize seu currículo e destaque-se"
            onButtonClick={() => handleNavigate('/page/cvdigital')}
          />

          {/* Card 2 – Pesquisa de satisfação */}
          <ActionCard
            title="Sua opinião é essencial para melhorarmos!"
            description="Responda nossa rápida pesquisa de satisfação e nos ajude a oferecer um serviço ainda melhor."
            buttonText="Responder"
            onButtonClick={() => setIsPesquisaModalOpen(true)}
          />
        </div>

        {/* 4️⃣ CANAL DE DENÚNCIAS - Abaixo dos ActionCards */}
        {hasCanalDenunciaConfig && <CanalDenuncias />}

        {/* Modal de Pesquisa de Satisfação */}
        <PesquisaSatisfacaoModal
          open={isPesquisaModalOpen}
          onOpenChange={setIsPesquisaModalOpen}
        />
      </div>
    </div>
  )
}
