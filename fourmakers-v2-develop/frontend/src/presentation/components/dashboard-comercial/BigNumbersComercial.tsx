import { useState } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  selectKpiSaude,
  selectKpiAlcance,
  selectFiltros,
  selectEvidenciasPorIndicador,
  selectAgendaRealizadaDetalhe,
  selectAgendaRealizadaDetalheCarregando,
  selectAgendaRealizadaDetalheErro,
  buscarAgendaRealizadaDetalhe,
  selectClientesImpactadosDetalhe,
  selectClientesImpactadosDetalheCarregando,
  selectClientesImpactadosDetalheErro,
  buscarClientesImpactadosDetalhe,
  selectAgendaSemInteracaoDetalhe,
  selectAgendaSemInteracaoDetalheCarregando,
  selectAgendaSemInteracaoDetalheErro,
  buscarAgendaSemInteracaoDetalhe,
  selectAcoesEmAtrasoDetalhe,
  selectAcoesEmAtrasoDetalheCarregando,
  selectAcoesEmAtrasoDetalheErro,
  buscarAcoesEmAtrasoDetalhe,
  selectCategoriaComInteracaoDetalhe,
  selectCategoriaComInteracaoDetalheCarregando,
  selectCategoriaComInteracaoDetalheErro,
  buscarCategoriaComInteracaoDetalhe,
  selectGestoresImpactadosDetalhe,
  selectGestoresImpactadosDetalheCarregando,
  selectGestoresImpactadosDetalheErro,
  buscarGestoresImpactadosDetalhe,
} from '@app/store/slices/dashboardComercialSlice'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { DrilldownSheet } from './DrilldownSheet'
import { DrilldownAgendasRealizadasModal } from './DrilldownAgendasRealizadasModal'
import { DrilldownClientesImpactadosModal } from './DrilldownClientesImpactadosModal'
import { DrilldownAgendaSemInteracaoModal } from './DrilldownAgendaSemInteracaoModal'
import { DrilldownAcoesEmAtrasoModal } from './DrilldownAcoesEmAtrasoModal'
import { DrilldownCategoriaComInteracaoModal } from './DrilldownCategoriaComInteracaoModal'
import { DrilldownGestoresImpactadosModal } from './DrilldownGestoresImpactadosModal'
import {
  Users,
  PersonRemove,
  AlertTriangle,
  Target,
  CheckCircle2,
  TrendingUp,
} from '@/components/ui/system-icons'
import type { IdIndicador } from '@shared/types/dashboardComercialTypes'
import { COMO_CALCULAMOS } from '@shared/utils/dashboardComercialMocks'

/** Esquema de cores alinhado ao Dashboard Jornadas (Skills) */
const GRADIENTES_KPI = {
  blue: 'bg-gradient-to-br from-blue-500 to-blue-600',
  indigo: 'bg-gradient-to-br from-indigo-500 to-indigo-600',
  emerald: 'bg-gradient-to-br from-emerald-500 to-emerald-600',
  rose: 'bg-gradient-to-br from-rose-500 to-rose-600',
  amber: 'bg-gradient-to-br from-amber-500 to-amber-600',
  teal: 'bg-gradient-to-br from-teal-500 to-teal-600',
} as const

interface KpiCardProps {
  titulo: string
  valor: number
  icone: React.ReactNode
  gradientClass: string
  sufixo?: string
  emBreve?: boolean
  onClick: () => void
  /** Quando true, card não abre drilldown (entrega em validação). */
  desabilitado?: boolean
}

function KpiCard({ titulo, valor, icone, gradientClass, sufixo = '', emBreve = false, onClick, desabilitado = false }: KpiCardProps) {
  const valorExibido = emBreve ? 'Em Breve' : (Number.isFinite(valor) ? `${valor}${sufixo}` : '—')
  return (
    <button
      type="button"
      onClick={desabilitado ? undefined : onClick}
      className={`flex h-32 w-full flex-col justify-between rounded-lgToken p-lg text-left text-white shadow-softToken duration-200 ${gradientClass} ${desabilitado ? 'cursor-not-allowed opacity-90' : 'transition-transform hover:scale-105 hover:shadow-cardHoverToken'}`}
    >
      <div className="flex items-start justify-between">
        <h3 className="text-sm font-medium uppercase tracking-wide text-white/95">{titulo}</h3>
        <div className="text-white/90">{icone}</div>
      </div>
      <p className="mt-2 text-4xl font-bold tabular-nums text-white">{valorExibido}</p>
    </button>
  )
}

export function BigNumbersComercial() {
  const dispatch = useAppDispatch()
  const user = useAppSelector((state) => state.auth.user)
  const token = useAppSelector((state) => state.auth.token)
  const kpiSaude = useAppSelector(selectKpiSaude)
  const kpiAlcance = useAppSelector(selectKpiAlcance)
  const filtros = useAppSelector(selectFiltros)
  const getEvidencias = useAppSelector(selectEvidenciasPorIndicador)
  const agendaRealizadaDetalhe = useAppSelector(selectAgendaRealizadaDetalhe)
  const agendaRealizadaDetalheCarregando = useAppSelector(selectAgendaRealizadaDetalheCarregando)
  const agendaRealizadaDetalheErro = useAppSelector(selectAgendaRealizadaDetalheErro)
  const clientesImpactadosDetalhe = useAppSelector(selectClientesImpactadosDetalhe)
  const clientesImpactadosDetalheCarregando = useAppSelector(selectClientesImpactadosDetalheCarregando)
  const clientesImpactadosDetalheErro = useAppSelector(selectClientesImpactadosDetalheErro)
  const agendaSemInteracaoDetalhe = useAppSelector(selectAgendaSemInteracaoDetalhe)
  const agendaSemInteracaoDetalheCarregando = useAppSelector(selectAgendaSemInteracaoDetalheCarregando)
  const agendaSemInteracaoDetalheErro = useAppSelector(selectAgendaSemInteracaoDetalheErro)
  const acoesEmAtrasoDetalhe = useAppSelector(selectAcoesEmAtrasoDetalhe)
  const acoesEmAtrasoDetalheCarregando = useAppSelector(selectAcoesEmAtrasoDetalheCarregando)
  const acoesEmAtrasoDetalheErro = useAppSelector(selectAcoesEmAtrasoDetalheErro)
  const categoriaComInteracaoDetalhe = useAppSelector(selectCategoriaComInteracaoDetalhe)
  const categoriaComInteracaoDetalheCarregando = useAppSelector(selectCategoriaComInteracaoDetalheCarregando)
  const categoriaComInteracaoDetalheErro = useAppSelector(selectCategoriaComInteracaoDetalheErro)
  const gestoresImpactadosDetalhe = useAppSelector(selectGestoresImpactadosDetalhe)
  const gestoresImpactadosDetalheCarregando = useAppSelector(selectGestoresImpactadosDetalheCarregando)
  const gestoresImpactadosDetalheErro = useAppSelector(selectGestoresImpactadosDetalheErro)

  const [drilldownAberto, setDrilldownAberto] = useState(false)
  const [indicadorAberto, setIndicadorAberto] = useState<{
    id: IdIndicador
    titulo: string
  } | null>(null)

  /** Drilldown habilitado para Encontros Realizados (modal de agendas); demais indicadores usam Sheet. */
  const DRILLDOWN_HABILITADO = true

  const abrirDrilldown = (id: IdIndicador, titulo: string) => {
    if (id !== 'encontros-realizados' && id !== 'clientes-impactados' && id !== 'sem-interacao' && id !== 'acoes-em-atraso' && id !== 'gestores-impactados' && id !== 'categorias-com-interacao' && !DRILLDOWN_HABILITADO) return
    logUserAction('DashboardComercial', 'AbrirDrilldownIndicador', { idIndicador: id, titulo }, user)
    setIndicadorAberto({ id, titulo })
    setDrilldownAberto(true)
    if (id === 'encontros-realizados' && token) {
      dispatch(buscarAgendaRealizadaDetalhe({ token, filtros }))
      return
    }
    if (id === 'clientes-impactados' && token) {
      dispatch(buscarClientesImpactadosDetalhe({ token, filtros }))
      return
    }
    if (id === 'sem-interacao' && token) {
      dispatch(buscarAgendaSemInteracaoDetalhe({ token, filtros }))
      return
    }
    if (id === 'acoes-em-atraso' && token) {
      dispatch(buscarAcoesEmAtrasoDetalhe({ token, filtros }))
      return
    }
    if (id === 'gestores-impactados' && token) {
      dispatch(buscarGestoresImpactadosDetalhe({ token, filtros }))
      return
    }
    if (id === 'categorias-com-interacao' && token) {
      dispatch(buscarCategoriaComInteracaoDetalhe({ token, filtros }))
      return
    }
    if (!DRILLDOWN_HABILITADO) return
  }

  return (
    <>
      <div className="mb-6 grid grid-cols-1 gap-6 lg:grid-cols-2">
        <div>
          <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
            KPIs — Saúde
          </h3>
          <div className="grid grid-cols-1 gap-4">
            <KpiCard
              titulo="Encontros Realizados"
              valor={kpiSaude.encontrosRealizados}
              icone={<Users size={24} />}
              gradientClass={GRADIENTES_KPI.blue}
              onClick={() => abrirDrilldown('encontros-realizados', 'Encontros Realizados')}
              desabilitado={false}
            />
            <KpiCard
              titulo="Sem Interação"
              valor={kpiSaude.semInteracao}
              icone={<PersonRemove size={24} />}
              gradientClass={GRADIENTES_KPI.rose}
              onClick={() => abrirDrilldown('sem-interacao', 'Sem Interação')}
            />
            <KpiCard
              titulo="Ações em Atraso"
              valor={kpiSaude.acoesEmAtraso}
              icone={<AlertTriangle size={24} />}
              gradientClass={GRADIENTES_KPI.amber}
              onClick={() => abrirDrilldown('acoes-em-atraso', 'Ações em Atraso')}
            />
          </div>
        </div>

        <div>
          <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
            KPIs — Alcance
          </h3>
          <div className="grid grid-cols-1 gap-4">
            <KpiCard
              titulo="Clientes Impactados"
              valor={kpiAlcance.clientesImpactados}
              icone={<Target size={24} />}
              gradientClass={GRADIENTES_KPI.indigo}
              onClick={() => abrirDrilldown('clientes-impactados', 'Clientes Impactados')}
              desabilitado={!DRILLDOWN_HABILITADO}
            />
            <KpiCard
              titulo="Gestores Impactados"
              valor={kpiAlcance.gestoresImpactados}
              icone={<CheckCircle2 size={24} />}
              gradientClass={GRADIENTES_KPI.emerald}
              onClick={() => abrirDrilldown('gestores-impactados', 'Gestores Impactados')}
              desabilitado={!DRILLDOWN_HABILITADO}
            />
            <KpiCard
              titulo="Categorias com Interação"
              valor={kpiAlcance.categoriasComInteracao}
              icone={<TrendingUp size={24} />}
              gradientClass={GRADIENTES_KPI.teal}
              onClick={() => abrirDrilldown('categorias-com-interacao', 'Categorias com Interação')}
            />
          </div>
        </div>
      </div>

      {indicadorAberto?.id === 'encontros-realizados' ? (
        <DrilldownAgendasRealizadasModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['encontros-realizados']}
          filtrosAtivos={filtros}
          dadosAgendaRealizadaDetalhe={agendaRealizadaDetalhe}
          agendaRealizadaDetalheCarregando={agendaRealizadaDetalheCarregando}
          agendaRealizadaDetalheErro={agendaRealizadaDetalheErro}
        />
      ) : indicadorAberto?.id === 'clientes-impactados' ? (
        <DrilldownClientesImpactadosModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['clientes-impactados']}
          filtrosAtivos={filtros}
          dadosClientesImpactados={clientesImpactadosDetalhe}
          carregando={clientesImpactadosDetalheCarregando}
          erro={clientesImpactadosDetalheErro}
        />
      ) : indicadorAberto?.id === 'sem-interacao' ? (
        <DrilldownAgendaSemInteracaoModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['sem-interacao']}
          filtrosAtivos={filtros}
          dados={agendaSemInteracaoDetalhe}
          carregando={agendaSemInteracaoDetalheCarregando}
          erro={agendaSemInteracaoDetalheErro}
        />
      ) : indicadorAberto?.id === 'acoes-em-atraso' ? (
        <DrilldownAcoesEmAtrasoModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['acoes-em-atraso']}
          filtrosAtivos={filtros}
          dados={acoesEmAtrasoDetalhe}
          carregando={acoesEmAtrasoDetalheCarregando}
          erro={acoesEmAtrasoDetalheErro}
        />
      ) : indicadorAberto?.id === 'categorias-com-interacao' ? (
        <DrilldownCategoriaComInteracaoModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['categorias-com-interacao']}
          filtrosAtivos={filtros}
          dados={categoriaComInteracaoDetalhe}
          carregando={categoriaComInteracaoDetalheCarregando}
          erro={categoriaComInteracaoDetalheErro}
        />
      ) : indicadorAberto?.id === 'gestores-impactados' ? (
        <DrilldownGestoresImpactadosModal
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS['gestores-impactados']}
          filtrosAtivos={filtros}
          dados={gestoresImpactadosDetalhe}
          carregando={gestoresImpactadosDetalheCarregando}
          erro={gestoresImpactadosDetalheErro}
        />
      ) : indicadorAberto ? (
        <DrilldownSheet
          open={drilldownAberto}
          onOpenChange={(open) => {
            setDrilldownAberto(open)
            if (!open) setIndicadorAberto(null)
          }}
          idIndicador={indicadorAberto.id}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS[indicadorAberto.id]}
          dadosEvidencia={getEvidencias(indicadorAberto.id)}
          filtrosAtivos={filtros}
          emBreve
        />
      ) : null}
    </>
  )
}
