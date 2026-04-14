import { useEffect } from 'react'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { buscarAgendaGestor, limparAgenda } from '@app/store/slices/agendaGestorSlice'

export function useAgendaGestor(codInternoColaborador: string) {
  const dispatch = useAppDispatch()
  const { agenda, status, error } = useAppSelector((state) => state.agendaGestor)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    if (token && codInternoColaborador && codInternoColaborador.trim() !== '') {
      dispatch(buscarAgendaGestor({ token, codInternoColaborador }))
    }

    return () => {
      dispatch(limparAgenda())
    }
  }, [dispatch, token, codInternoColaborador])

  const refetch = () => {
    if (token && codInternoColaborador) {
      dispatch(buscarAgendaGestor({ token, codInternoColaborador }))
    }
  }

  return {
    agenda,
    loading: status === 'loading',
    error,
    refetch,
  }
}
