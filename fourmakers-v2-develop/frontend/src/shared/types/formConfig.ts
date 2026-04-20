export interface FormFieldConfig {
  name: string
  visible: boolean
  required: boolean
}

export interface FormConfig {
  formId: string
  fields: FormFieldConfig[]
}

