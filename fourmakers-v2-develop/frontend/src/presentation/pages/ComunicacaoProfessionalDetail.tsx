import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import {
  ArrowLeft,
  Star,
  Briefcase,
  MapPin,
  Mail,
  Phone,
  Calendar,
} from 'lucide-react';
import type { Professional } from '@domain/entities/comunicacao';
import { useAppSelector } from '@app/store/hooks';
import { useComunicacaoProfissionais } from '@presentation/hooks/useComunicacaoProfissionais';
import { container } from '@core/di/container';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoProfissionaisApi } from '@data/api/ComunicacaoProfissionaisApi';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';
import teamsIcon from '@/assets/teams-icon.png';

export default function ComunicacaoProfessionalDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const token = useAppSelector((state) => state.auth.token);
  const [isFavorite, setIsFavorite] = useState(false);
  const [togglingFavorite, setTogglingFavorite] = useState(false);

  const professionalFromState = (location.state as { professional?: Professional } | null)
    ?.professional;
  const { profissionais, loading, loadProfissionais } = useComunicacaoProfissionais();
  const professionalFromList = id
    ? profissionais.find((p) => p.id === id)
    : undefined;

  const professional =
    professionalFromState?.id === id
      ? professionalFromState
      : professionalFromList;

  useEffect(() => {
    if (professional) {
      setIsFavorite(professional.favoritado === true);
    }
  }, [professional?.id, professional?.favoritado]);

  const handleToggleFavorite = useCallback(async () => {
    if (!token || !professional || togglingFavorite) return;
    const api = container.resolve<ComunicacaoProfissionaisApi>(DiTokens.comunicacaoProfissionaisApi);
    setTogglingFavorite(true);
    try {
      if (isFavorite) {
        await api.desfavoritar(token, professional.id);
        setIsFavorite(false);
        toast.success('Removido dos favoritos.');
      } else {
        await api.favoritar(token, professional.id);
        setIsFavorite(true);
        toast.success('Adicionado aos favoritos.');
      }
      await loadProfissionais();
    } catch {
      toast.error('Não foi possível atualizar o favorito.');
    } finally {
      setTogglingFavorite(false);
    }
  }, [token, professional, isFavorite, togglingFavorite, loadProfissionais]);

  const formatBirthDate = (birthDate?: string): string | null => {
    if (!birthDate) return null;
    const parts = birthDate.split('-');
    if (parts.length === 2)
      return `${parts[1].padStart(2, '0')}/${parts[0].padStart(2, '0')}`;
    if (parts.length === 3)
      return `${parts[2].padStart(2, '0')}/${parts[1].padStart(2, '0')}/${parts[0]}`;
    return null;
  };

  if (loading && !professionalFromState) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <Spinner className="h-8 w-8 text-muted-foreground" />
      </div>
    );
  }

  if (!professional) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold mb-2">
            Profissional não encontrado
          </h2>
          <Button
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Voltar
          </Button>
        </div>
      </div>
    );
  }

  const initials = professional.name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .substring(0, 2)
    .toUpperCase();
  const avatarSrc =
    professional.avatar && token
      ? professional.avatar.replace(/\$1/g, btoa(token))
      : professional.avatar ?? undefined;

  return (
    <div className="min-h-screen bg-background">
      <main className="container mx-auto px-4 py-6 max-w-4xl">
        <div className="transition-opacity">
          <Button
            variant="ghost"
            onClick={() => navigate(-1)}
            className="mb-6 gap-2"
          >
            <ArrowLeft className="w-4 h-4" />
            Voltar
          </Button>

          <Card className="overflow-hidden">
            <div
              className="h-48 bg-gradient-to-r from-purple-600 to-blue-600 relative"
              style={{
                backgroundImage: professional.coverImage
                  ? `url(${professional.coverImage})`
                  : undefined,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
              }}
            >
              <div className="absolute -bottom-12 left-6">
                <Avatar className="w-24 h-24 ring-4 ring-background">
                  <AvatarImage
                    src={avatarSrc}
                    alt={professional.name}
                  />
                  <AvatarFallback className="bg-gradient-to-br from-primary to-accent text-primary-foreground text-2xl font-bold">
                    {initials}
                  </AvatarFallback>
                </Avatar>
              </div>

              <Button
                variant="secondary"
                size="icon"
                className="absolute top-4 right-4 rounded-full"
                onClick={handleToggleFavorite}
                disabled={togglingFavorite}
              >
                {togglingFavorite ? (
                  <Spinner className="w-5 h-5 text-muted-foreground" />
                ) : (
                  <Star
                    className={`w-5 h-5 ${isFavorite ? 'fill-warning text-warning' : ''}`}
                  />
                )}
              </Button>
            </div>

            <CardContent className="pt-16 pb-6">
              <div className="space-y-4">
                <div>
                  <h1 className="text-3xl font-bold">{professional.name}</h1>
                  <div className="flex flex-wrap items-center gap-3 mt-3">
                    <Badge variant="outline" className="gap-1.5 py-1.5 px-3">
                      <Briefcase className="w-3.5 h-3.5" />
                      {professional.position}
                    </Badge>
                    <Badge variant="outline" className="gap-1.5 py-1.5 px-3">
                      <MapPin className="w-3.5 h-3.5" />
                      {professional.unit}
                    </Badge>
                    {professional.managerName && (
                      <Badge variant="secondary" className="gap-1.5 py-1.5 px-3">
                        Gestor: {professional.managerName}
                      </Badge>
                    )}
                    {professional.birthDate && (
                      <Badge variant="outline" className="gap-1.5 py-1.5 px-3">
                        <Calendar className="w-3.5 h-3.5" />
                        {formatBirthDate(professional.birthDate)}
                      </Badge>
                    )}
                  </div>
                </div>

                {professional.about && (
                  <div>
                    <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-2">
                      Sobre
                    </h3>
                    <p className="text-muted-foreground leading-relaxed">
                      {professional.about}
                    </p>
                  </div>
                )}

                {(professional.email ||
                  professional.phone ||
                  professional.birthDate) && (
                  <div className="space-y-2">
                    <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-2">
                      Contato e dados
                    </h3>
                    <div className="space-y-2">
                      {professional.birthDate &&
                        formatBirthDate(professional.birthDate) && (
                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <Calendar className="w-4 h-4" />
                            <span>
                              Data de nascimento:{' '}
                              {formatBirthDate(professional.birthDate)}
                            </span>
                          </div>
                        )}
                      {professional.email && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Mail className="w-4 h-4" />
                          <span>{professional.email}</span>
                        </div>
                      )}
                      {professional.phone && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Phone className="w-4 h-4" />
                          <span>{professional.phone}</span>
                        </div>
                      )}
                    </div>
                  </div>
                )}

                <Button
                  className="w-full gap-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700"
                  onClick={() => {
                    if (professional.email) {
                      const teamsChatUrl = `https://teams.microsoft.com/l/chat/0/0?users=${encodeURIComponent(professional.email)}`;
                      window.open(teamsChatUrl, '_blank', 'noopener,noreferrer');
                    } else {
                      toast.error('E-mail não disponível para abrir conversa no Teams.');
                    }
                  }}
                  disabled={!professional.email}
                >
                  <img src={teamsIcon} alt="" className="w-4 h-4 shrink-0" aria-hidden />
                  Enviar Mensagem
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
