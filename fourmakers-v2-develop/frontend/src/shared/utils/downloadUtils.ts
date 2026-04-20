/**
 * Dispara download de um Blob (ex.: arquivo xlsx retornado pela API como binário).
 */
export function downloadBlob(blob: Blob, filename: string): void {
  const name = filename.endsWith('.xlsx') ? filename : `${filename}.xlsx`;
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = name;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

/**
 * Decodifica string base64 e dispara download do arquivo.
 * Usado para relatórios xlsx retornados em base64 pela API.
 */
export function downloadBase64File(base64: string, filename: string, mimeType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'): void {
  const binary = atob(base64);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  const blob = new Blob([bytes], { type: mimeType });
  downloadBlob(blob, filename);
}
