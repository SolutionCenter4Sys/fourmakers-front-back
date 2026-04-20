import { Camera, MapPin, Briefcase, Mail, Phone, Edit } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Icon } from "@/components/ui/icon";
import logoFourmakers from "@/assets/logo-fourmakers.svg";
import type { ReactElement } from "react";
import type { ColaboradorProfile360 } from "@domain/entities/Profile360";
import { toast } from "sonner";

interface ProfileHeroProps {
  dados?: ColaboradorProfile360;
  token?: string | null;
  onEditData?: () => void;
  readOnly?: boolean;
}

const DEFAULT_AVATAR_URL =
  "https://storage.googleapis.com/flutterflow-io-6f20.appspot.com/projects/fourmakers-2-vr1q98/assets/wntmgxmbqzt5/avatar-inclinado.png";

const encodeTokenBase64 = (token: string): string => {
  try {
    return btoa(unescape(encodeURIComponent(token)));
  } catch {
    return btoa(token);
  }
};

const normalizePhotoUrl = (url: string | undefined, token?: string | null): string => {
  const trimmedUrl = url?.trim();
  if (!trimmedUrl) return DEFAULT_AVATAR_URL;
  if (!token) return trimmedUrl;
  const encodedToken = encodeTokenBase64(token);
  return trimmedUrl.replace("$1", encodeURIComponent(encodedToken));
};

const formatPhone = (value?: string | null): string => {
  const raw = value?.trim();
  if (!raw) return "";
  const digits = raw.replace(/\D/g, "");
  if (digits.length === 11) return digits.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3");
  if (digits.length === 10) return digits.replace(/(\d{2})(\d{4})(\d{4})/, "($1) $2-$3");
  return raw;
};

const normalizeValue = (value?: string | null): string => {
  const trimmed = value?.trim() ?? "";
  if (!trimmed) return "";
  const lowered = trimmed.toLowerCase();
  if (lowered === "null" || lowered === "undefined" || lowered === "não informado" || lowered === "nao informado") {
    return "";
  }
  return trimmed;
};

export const ProfileHero = ({ dados, token, onEditData, readOnly = false }: ProfileHeroProps) => {
  const iniciais = dados?.nomeCompleto
    ?.split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase()
    .slice(0, 2) || "?";

  const diretoria = normalizeValue(dados?.diretoria?.diretoria);
  const cargo = normalizeValue(dados?.cargo?.cargo) || diretoria;
  const cidade = normalizeValue(dados?.endereco?.cidade);
  const estado = normalizeValue(dados?.endereco?.estado);
  const cidadeEstado = [cidade, estado].filter(Boolean).join("/");
  const telefone = formatPhone(normalizeValue(dados?.contatoPrincipal));
  const email = normalizeValue(dados?.email);
  const emailAlternativo = normalizeValue(dados?.emailAlternativo);
  const linkedinRaw = normalizeValue(dados?.urlLinkedin);
  const linkedin = linkedinRaw
    ? `linkedin.com/in/${linkedinRaw.replace(/^https?:\/\/(www\.)?linkedin\.com\/in\//i, "").replace(/^linkedin\.com\/in\//i, "")}`
    : "";
  const fotoUrl = normalizePhotoUrl(dados?.urlFoto, token);
  const status = dados?.flagAtivo ? "Ativo" : "Inativo";

  const copyToClipboard = async (value: string, label: string) => {
    try {
      await navigator.clipboard.writeText(value);
      toast.success(`${label} copiado para a área de transferência.`);
    } catch {
      toast.error(`Não foi possível copiar ${label.toLowerCase()}.`);
    }
  };

  const infoItems = [
    cidadeEstado
      ? { key: "cidadeEstado", icon: <MapPin className="h-4 w-4" />, value: cidadeEstado, copyLabel: "" }
      : null,
    telefone
      ? { key: "telefone", icon: <Phone className="h-4 w-4" />, value: telefone, copyLabel: "Telefone" }
      : null,
    email
      ? { key: "email", icon: <Mail className="h-4 w-4" />, value: email, copyLabel: "E-mail" }
      : null,
    emailAlternativo
      ? { key: "emailAlternativo", icon: <Mail className="h-4 w-4" />, value: emailAlternativo, copyLabel: "E-mail alternativo" }
      : null,
    linkedin
      ? { key: "linkedin", icon: <Briefcase className="h-4 w-4" />, value: linkedin, copyLabel: "LinkedIn" }
      : null,
  ].filter(Boolean) as Array<{ key: string; icon: ReactElement; value: string; copyLabel: string }>;

  return (
    <div className="relative overflow-hidden rounded-2xl bg-brand-gradient text-inverseText shadow-softToken p-6 md:p-8">
      <div className="absolute top-5 right-5">
        <img 
          src={logoFourmakers} 
          alt="FourMakers" 
          className="h-8 w-auto brightness-0 invert opacity-90"
        />
      </div>
      
      <div className="flex flex-col items-center gap-6 md:flex-row md:items-center md:gap-8">
        <div className="relative mx-auto md:mx-0">
          <Avatar className="h-32 w-32 border-2 border-white/80 shadow-softToken">
            <AvatarImage src={fotoUrl} className="object-cover object-center" />
            <AvatarFallback className="text-3xl font-semibold text-white bg-white/20">
              {iniciais}
            </AvatarFallback>
          </Avatar>
          {!readOnly && (
            <Button
              size="icon"
              variant="secondary"
                className="absolute bottom-1 right-1 h-9 w-9 rounded-full shadow-softToken bg-white/20 text-white border-white/30 hover:bg-white/30"
              >
                <Camera className="h-4 w-4" />
              </Button>
          )}
        </div>

        <div className="flex-1 min-w-0 space-y-3">
          <div className="space-y-1">
            <div className="flex flex-wrap items-center gap-3">
              <h1 className="text-4xl font-bold tracking-tight text-white">
                {dados?.nomeCompleto || "Carregando..."}
              </h1>
              <Badge className="bg-white/20 text-white border-white/30 text-sm">
                {status}
              </Badge>
              <Button
                variant="ghost"
                size="icon"
                className="h-9 w-9 text-white hover:bg-white/20 hover:text-white"
                aria-label="Editar dados pessoais"
                onClick={onEditData}
              >
                <Edit className="h-5 w-5" />
              </Button>
            </div>
            {cargo && (
              <p className="text-base text-white/90 font-medium">
                {cargo}
              </p>
            )}
          </div>

          <div className="flex flex-col gap-2 text-sm text-white/90 md:flex-row md:flex-wrap md:items-center md:gap-x-5 md:gap-y-2">
            {diretoria && (
              <div className="inline-flex items-center gap-1.5">
                <Briefcase className="h-4 w-4" />
                <span>{diretoria}</span>
              </div>
            )}
            {infoItems.map((item) => (
              <div key={item.key} className="inline-flex items-center gap-1.5">
                {item.icon}
                <span>{item.value}</span>
                {item.copyLabel && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 text-white/90 hover:bg-white/20 hover:text-white"
                    aria-label={`Copiar ${item.copyLabel}`}
                    onClick={() => copyToClipboard(item.value, item.copyLabel)}
                  >
                    <Icon name="content_copy" size={14} />
                  </Button>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};
