/**
 * Indicador de força de senha e checklist de requisitos.
 * Usado em Login e candidatura pública (cadastro).
 */
interface PasswordStrengthProps {
  password: string;
}

export function PasswordStrength({ password }: PasswordStrengthProps) {
  const calcularForca = (
    senha: string
  ): { nivel: number; label: string; barClass: string; labelClass: string } => {
    if (!senha) {
      return { nivel: 0, label: '', barClass: '', labelClass: '' };
    }

    let forca = 0;
    const criterios = {
      comprimento: senha.length >= 8,
      minuscula: /[a-z]/.test(senha),
      maiuscula: /[A-Z]/.test(senha),
      numero: /[0-9]/.test(senha),
      especial: /[^a-zA-Z0-9]/.test(senha),
    };

    if (criterios.comprimento) forca++;
    if (criterios.minuscula) forca++;
    if (criterios.maiuscula) forca++;
    if (criterios.numero) forca++;
    if (criterios.especial) forca++;

    if (forca <= 1) {
      return { nivel: 1, label: 'Muito fraca', barClass: 'bg-destructive', labelClass: 'text-destructive' };
    }
    if (forca <= 2) {
      return { nivel: 2, label: 'Fraca', barClass: 'bg-destructive/80', labelClass: 'text-destructive' };
    }
    if (forca <= 3) {
      return { nivel: 3, label: 'Média', barClass: 'bg-warning', labelClass: 'text-warning' };
    }
    if (forca <= 4) {
      return { nivel: 4, label: 'Forte', barClass: 'bg-success', labelClass: 'text-success' };
    }
    return { nivel: 5, label: 'Muito forte', barClass: 'bg-success', labelClass: 'text-success' };
  };

  const forca = calcularForca(password);

  if (!password) return null;

  return (
    <div className="space-y-2 mt-2">
      <div className="flex items-center gap-2">
        <div className="flex-1 h-2 bg-muted rounded-full overflow-hidden">
          <div
            className={`h-full transition-all duration-300 ${forca.barClass}`}
            style={{ width: `${(forca.nivel / 5) * 100}%` }}
          />
        </div>
        {forca.label && <span className={`text-xs font-medium ${forca.labelClass}`}>{forca.label}</span>}
      </div>
      <div className="text-xs text-muted-foreground space-y-1">
        <p className="font-medium">A senha deve conter:</p>
        <ul className="list-disc list-inside space-y-0.5 ml-2">
          <li className={password.length >= 8 ? 'text-success' : ''}>Pelo menos 8 caracteres</li>
          <li className={/[a-z]/.test(password) ? 'text-success' : ''}>Uma letra minúscula</li>
          <li className={/[A-Z]/.test(password) ? 'text-success' : ''}>Uma letra maiúscula</li>
          <li className={/[0-9]/.test(password) ? 'text-success' : ''}>Um número</li>
          <li className={/[^a-zA-Z0-9]/.test(password) ? 'text-success' : ''}>Um caractere especial</li>
        </ul>
      </div>
    </div>
  );
}
