import type {
  ExperienceMock,
  EducationMock,
  CertificationMock,
  VisaMock,
  PassportMock,
  SkillCategoryMock,
} from '../mocks/profileMock'
import {
  experiencesMock,
  educationsMock,
  certificationsMock,
  visasMock,
  passportsMock,
  skillCategoriesMock,
} from '../mocks/profileMock'

export class ProfileApi {
  async getExperiences(): Promise<ExperienceMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...experiencesMock]
  }

  async getEducations(): Promise<EducationMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...educationsMock]
  }

  async getCertifications(): Promise<CertificationMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...certificationsMock]
  }

  async getVisas(): Promise<VisaMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...visasMock]
  }

  async getPassports(): Promise<PassportMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...passportsMock]
  }

  async getSkillCategories(): Promise<SkillCategoryMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...skillCategoriesMock]
  }
}

