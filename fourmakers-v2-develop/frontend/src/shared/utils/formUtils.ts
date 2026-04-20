/**
 * Utilitários para formulários
 */

/**
 * Remove um erro específico do objeto de erros
 * @param errors Objeto de erros atual
 * @param campo Nome do campo a ter o erro removido
 * @returns Novo objeto de erros sem o campo especificado
 */
export function clearFieldError(
  errors: { [campo: string]: string },
  campo: string
): { [campo: string]: string } {
  const newErrors = { ...errors }
  delete newErrors[campo]
  return newErrors
}

/**
 * Remove múltiplos erros do objeto de erros
 * @param errors Objeto de erros atual
 * @param campos Array com os nomes dos campos a ter os erros removidos
 * @returns Novo objeto de erros sem os campos especificados
 */
export function clearFieldErrors(
  errors: { [campo: string]: string },
  campos: string[]
): { [campo: string]: string } {
  const newErrors = { ...errors }
  campos.forEach((campo) => {
    delete newErrors[campo]
  })
  return newErrors
}
