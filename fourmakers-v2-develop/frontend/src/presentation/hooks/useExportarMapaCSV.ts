import { useCallback } from 'react'

import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

interface NoFlat {
  id: string
  parentId: string | null
  profileId: string | null
  profileName: string
  employeeId: string | null
  employeeName: string
  employeeEmail: string
  managerCode: string
  departmentId: string
  departmentName: string
  isCLevel: boolean
  isExternal: boolean
  wasConnected: boolean
}

export function useExportarMapaCSV() {
  const converterParaFlat = useCallback((no: NoMapaRelacionamento, parentId: string | null = null): NoFlat[] => {
    const resultado: NoFlat[] = [
      {
        id: no.id,
        parentId,
        profileId: no.profileId,
        profileName: no.profileName || '',
        employeeId: no.employeeId,
        employeeName: no.employeeName || '',
        employeeEmail: no.employeeEmail || '',
        managerCode: no.managerCode || '',
        departmentId: no.departmentId || '',
        departmentName: no.departmentName || '',
        isCLevel: no.isCLevel || false,
        isExternal: no.isExternal || false,
        wasConnected: no.wasConnected || false,
      },
    ]

    no.children.forEach((filho) => {
      resultado.push(...converterParaFlat(filho, no.id))
    })

    return resultado
  }, [])

  const exportarCSV = useCallback(
    (estrutura: NoMapaRelacionamento, nomeArquivo: string = 'mapa-relacionamento.csv') => {
      const flat = converterParaFlat(estrutura)

      const headers = [
        'id',
        'parentId',
        'profileId',
        'profileName',
        'employeeId',
        'employeeName',
        'employeeEmail',
        'managerCode',
        'departmentId',
        'departmentName',
        'isCLevel',
        'isExternal',
        'wasConnected',
      ]

      const csvContent = [
        headers.join(','),
        ...flat.map((row) =>
          headers
            .map((header) => {
              const value = row[header as keyof NoFlat]
              return value === null ? '' : `"${String(value).replace(/"/g, '""')}"`
            })
            .join(','),
        ),
      ].join('\n')

      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
      const link = document.createElement('a')
      const url = URL.createObjectURL(blob)

      link.setAttribute('href', url)
      link.setAttribute('download', nomeArquivo)
      link.style.visibility = 'hidden'

      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
    },
    [converterParaFlat],
  )

  return { exportarCSV }
}
