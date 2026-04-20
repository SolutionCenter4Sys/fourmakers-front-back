import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ProfileApi } from '@data/api/ProfileApi'
import type {
  ExperienceMock,
  EducationMock,
  CertificationMock,
  VisaMock,
  PassportMock,
  SkillCategoryMock,
} from '@data/mocks/profileMock'

export const useProfileExperiences = () => {
  const [experiences, setExperiences] = useState<ExperienceMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ProfileApi)
        const data = await api.getExperiences()
        setExperiences(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { experiences, loading, error }
}

export const useProfileEducations = () => {
  const [educations, setEducations] = useState<EducationMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ProfileApi)
        const data = await api.getEducations()
        setEducations(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { educations, loading, error }
}

export const useProfileCertifications = () => {
  const [certifications, setCertifications] = useState<CertificationMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ProfileApi)
        const data = await api.getCertifications()
        setCertifications(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { certifications, loading, error }
}

export const useProfileDocuments = () => {
  const [visas, setVisas] = useState<VisaMock[]>([])
  const [passports, setPassports] = useState<PassportMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ProfileApi)
        const [visasData, passportsData] = await Promise.all([
          api.getVisas(),
          api.getPassports(),
        ])
        setVisas(visasData)
        setPassports(passportsData)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { visas, passports, loading, error }
}

export const useProfileSkills = () => {
  const [skillCategories, setSkillCategories] = useState<SkillCategoryMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ProfileApi)
        const data = await api.getSkillCategories()
        setSkillCategories(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { skillCategories, loading, error }
}

