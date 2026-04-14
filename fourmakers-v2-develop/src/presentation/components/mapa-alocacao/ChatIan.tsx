import { useState, useEffect, useCallback, useRef } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { X, Send, Trash2, Edit, Minimize2, Maximize2 } from "@/components/ui/system-icons";
import { Icon } from "@/components/ui/icon";
import { useAppSelector } from "@/app/store/hooks";
import { container } from "@/core/di/container";
import { InserirQuestaoUseCase } from "@domain/usecases/InserirQuestaoUseCase";
import { ListarChatsUseCase } from "@domain/usecases/ListarChatsUseCase";
import { ListarQuestoesUseCase } from "@domain/usecases/ListarQuestoesUseCase";
import { RemoverChatUseCase } from "@domain/usecases/RemoverChatUseCase";
import { InserirFeedbackBotUseCase } from "@domain/usecases/InserirFeedbackBotUseCase";
import type { Chat, Questao } from "@domain/entities/BotFourmakers";
import { toast } from "sonner";
import { formatDistanceToNow } from "date-fns";
import { ptBR } from "date-fns/locale";

// Avatar do Ian - imagem deve estar em public/assets/ ou src/assets/
const IAN_AVATAR = "/assets/avatarFourSocial-6764c954-85b9-4e11-ac34-72e2f3e93aa2.png";
const MENSAGEM_INICIAL = "Olá, como posso te ajudar hoje?";

export const ChatIan = () => {
  const { token } = useAppSelector((state) => state.auth);
  const [isOpen, setIsOpen] = useState(false);
  const [isExpanded, setIsExpanded] = useState(false);
  const [mensagem, setMensagem] = useState("");
  const [chats, setChats] = useState<Chat[]>([]);
  const [chatAtual, setChatAtual] = useState<number | null>(null);
  const [questoes, setQuestoes] = useState<Questao[]>([]);
  const [loading, setLoading] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [loadingChats, setLoadingChats] = useState(false);
  const [feedbackModalOpen, setFeedbackModalOpen] = useState(false);
  const [feedbackQuestaoId, setFeedbackQuestaoId] = useState<number | null>(null);
  const [feedbackTipo, setFeedbackTipo] = useState<boolean | null>(null);
  const [feedbackComentario, setFeedbackComentario] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  // Scroll para o final das mensagens
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    if (isOpen && questoes.length > 0) {
      scrollToBottom();
    }
  }, [isOpen, questoes]);

  // Carregar chats recentes
  const carregarChats = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingChats(true);
      const useCase = container.resolve(ListarChatsUseCase);
      const response = await useCase.execute(token);

      if (response.sucesso && response.retorno) {
        setChats(response.retorno);
      }
    } catch (error) {
      console.error("Erro ao carregar chats:", error);
    } finally {
      setLoadingChats(false);
    }
  }, [token]);

  // Carregar questões de um chat
  const carregarQuestoes = useCallback(async (chatId: number) => {
    if (!token) return;

    try {
      setLoading(true);
      const useCase = container.resolve(ListarQuestoesUseCase);
      const response = await useCase.execute(token, { chatId });

      if (response.sucesso && response.retorno) {
        setQuestoes(response.retorno);
        setChatAtual(chatId);
      }
    } catch (error) {
      console.error("Erro ao carregar questões:", error);
      toast.error("Erro ao carregar mensagens");
    } finally {
      setLoading(false);
    }
  }, [token]);

  // Remover chat
  const removerChat = useCallback(async (chatId: number) => {
    if (!token) return;

    try {
      const useCase = container.resolve(RemoverChatUseCase);
      const response = await useCase.execute(token, { chatId });

      if (response.sucesso) {
        setChats(chats.filter(c => c.id !== chatId));
        if (chatAtual === chatId) {
          setChatAtual(null);
          setQuestoes([]);
        }
        toast.success("Conversa removida com sucesso");
      } else {
        toast.error(response.mensagem || "Erro ao remover conversa");
      }
    } catch (error) {
      console.error("Erro ao remover chat:", error);
      toast.error("Erro ao remover conversa");
    }
  }, [token, chats, chatAtual]);

  // Enviar mensagem
  const enviarMensagem = useCallback(async () => {
    if (!token || !mensagem.trim() || enviando) return;

    const mensagemTexto = mensagem.trim();
    setMensagem("");
    setEnviando(true);

    try {
      const useCase = container.resolve(InserirQuestaoUseCase);
      const response = await useCase.execute(token, {
        questao: mensagemTexto,
        chatId: chatAtual,
      });

      if (response.sucesso && response.retorno) {
        setQuestoes(response.retorno);
        const novoChatId = response.retorno[0]?.chatId;
        if (novoChatId) {
          setChatAtual(novoChatId);
          // Recarregar chats para atualizar a lista
          await carregarChats();
        }
      } else {
        toast.error(response.mensagem || "Erro ao enviar mensagem");
      }
    } catch (error) {
      console.error("Erro ao enviar mensagem:", error);
      toast.error("Erro ao enviar mensagem");
    } finally {
      setEnviando(false);
      inputRef.current?.focus();
    }
  }, [token, mensagem, chatAtual, enviando, carregarChats]);

  // Abrir modal de feedback
  const abrirModalFeedback = useCallback((questaoId: number, feedback: boolean) => {
    setFeedbackQuestaoId(questaoId);
    setFeedbackTipo(feedback);
    setFeedbackComentario("");
    setFeedbackModalOpen(true);
  }, []);

  // Enviar feedback
  const enviarFeedback = useCallback(async () => {
    if (!token || feedbackQuestaoId === null || feedbackTipo === null) return;

    try {
      const useCase = container.resolve(InserirFeedbackBotUseCase);
      const response = await useCase.execute(token, {
        questaoId: feedbackQuestaoId,
        comentario: feedbackComentario.trim(),
        feedback: feedbackTipo,
      });

      if (response.sucesso) {
        // Atualizar a questão localmente
        setQuestoes(questoes.map(q => 
          q.id === feedbackQuestaoId ? { ...q, feedback: feedbackTipo } : q
        ));
        setFeedbackModalOpen(false);
        setFeedbackComentario("");
        toast.success("Feedback enviado com sucesso!");
      } else {
        toast.error(response.mensagem || "Erro ao enviar feedback");
      }
    } catch (error) {
      console.error("Erro ao enviar feedback:", error);
      toast.error("Erro ao enviar feedback");
    }
  }, [token, feedbackQuestaoId, feedbackTipo, feedbackComentario, questoes]);

  // Copiar mensagem
  const copiarMensagem = (texto: string) => {
    navigator.clipboard.writeText(texto);
    toast.success("Mensagem copiada!");
  };

  // Abrir chat
  const handleAbrirChat = useCallback(() => {
    setIsOpen(true);
    carregarChats();
    if (!chatAtual) {
      // Se não há chat atual, mostrar mensagem inicial
      setQuestoes([{
        id: 0,
        chatId: 0,
        mensagem: MENSAGEM_INICIAL,
        criadoEm: new Date().toISOString(),
        chat: { id: 0, mensagem: "", criadoEm: "" },
        feedback: null,
        tipo: 2, // Resposta do bot
        queryHabilidades: "",
      }]);
    } else {
      // Se há chat atual, carregar suas questões
      carregarQuestoes(chatAtual);
    }
  }, [chatAtual, carregarChats, carregarQuestoes]);

  // Selecionar chat
  const handleSelecionarChat = useCallback((chat: Chat) => {
    setChatAtual(chat.id);
    carregarQuestoes(chat.id);
  }, [carregarQuestoes]);

  // Novo chat
  const handleNovoChat = useCallback(() => {
    setChatAtual(null);
    setMensagem("");
    setQuestoes([{
      id: 0,
      chatId: 0,
      mensagem: MENSAGEM_INICIAL,
      criadoEm: new Date().toISOString(),
      chat: { id: 0, mensagem: "", criadoEm: "" },
      feedback: null,
      tipo: 2,
      queryHabilidades: "",
    }]);
    inputRef.current?.focus();
  }, []);

  // Formatar data relativa
  const formatarDataRelativa = (data: string) => {
    try {
      const date = new Date(data);
      return formatDistanceToNow(date, { addSuffix: true, locale: ptBR });
    } catch {
      return "";
    }
  };

  return (
    <>
      {/* FAB - Floating Action Button */}
      {!isOpen && (
        <button
          onClick={handleAbrirChat}
          className="fixed bottom-6 right-6 h-16 w-16 rounded-full shadow-2xl z-50 hover:scale-110 transition-transform p-0 overflow-hidden bg-transparent border-0 cursor-pointer flex items-center justify-center"
          style={{ 
            padding: 0,
            margin: 0,
            lineHeight: 0,
            boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.2)'
          }}
        >
          <img
            src={IAN_AVATAR}
            alt="Ian"
            className="rounded-full"
            style={{ 
              width: '100%',
              height: '100%',
              objectFit: 'cover',
              objectPosition: 'center 20%',
              display: 'block',
              margin: 0,
              padding: 0
            }}
          />
        </button>
      )}

      {/* Chat Modal */}
      {isOpen && (
        <div
          className={cn(
            "fixed z-50 transition-all duration-300",
            isExpanded
              ? "bottom-0 right-0 w-[90vw] max-w-[1200px] h-[90vh]"
              : "bottom-6 right-6 w-[400px] h-[600px]"
          )}
        >
          <div className="bg-background border border-border rounded-lg shadow-2xl flex flex-col h-full">
            {/* Header */}
            <div className="bg-black text-white p-4 rounded-t-lg flex items-center justify-between">
              <div className="flex items-center gap-3">
                <Avatar className="h-10 w-10">
                  <AvatarImage src={IAN_AVATAR} alt="Ian" />
                  <AvatarFallback>Ian</AvatarFallback>
                </Avatar>
                <div>
                  <div className="font-semibold">Ian</div>
                  <div className="text-xs text-green-400 flex items-center gap-1">
                    <span className="h-2 w-2 bg-green-400 rounded-full"></span>
                    Online
                  </div>
                </div>
              </div>
              <div className="flex items-center gap-2">
                <Button
                  variant="ghost"
                  size="icon"
                  className="text-white hover:bg-white/20"
                  onClick={() => setIsExpanded(!isExpanded)}
                >
                  {isExpanded ? (
                    <Minimize2 className="h-4 w-4" />
                  ) : (
                    <Maximize2 className="h-4 w-4" />
                  )}
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="text-white hover:bg-white/20"
                  onClick={() => {
                    setIsOpen(false);
                    setIsExpanded(false);
                  }}
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>

            <div className="flex flex-1 overflow-hidden">
              {/* Sidebar - Conversas Recentes (apenas quando expandido) */}
              {isExpanded && (
                <div className="w-80 border-r border-border flex flex-col">
                  <div className="p-4 border-b border-border flex items-center justify-between">
                    <h3 className="font-semibold">Recentes</h3>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={handleNovoChat}
                      className="h-8 w-8"
                    >
                      <Edit className="h-4 w-4" />
                    </Button>
                  </div>
                  <div className="flex-1 overflow-y-auto p-2">
                    {loadingChats ? (
                      <div className="text-center py-4 text-muted-foreground text-sm">
                        Carregando...
                      </div>
                    ) : chats.length === 0 ? (
                      <div className="text-center py-4 text-muted-foreground text-sm">
                        Nenhuma conversa recente
                      </div>
                    ) : (
                      chats.map((chat) => (
                        <div
                          key={chat.id}
                          className={cn(
                            "p-3 rounded-lg cursor-pointer hover:bg-muted transition-colors mb-2",
                            chatAtual === chat.id && "bg-muted"
                          )}
                          onClick={() => handleSelecionarChat(chat)}
                        >
                          <div className="flex items-start justify-between gap-2">
                            <p className="text-sm line-clamp-2 flex-1">
                              {chat.mensagem}
                            </p>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-6 w-6 flex-shrink-0"
                              onClick={(e) => {
                                e.stopPropagation();
                                removerChat(chat.id);
                              }}
                            >
                              <Trash2 className="h-3 w-3" />
                            </Button>
                          </div>
                          <div className="text-xs text-muted-foreground mt-1">
                            {formatarDataRelativa(chat.criadoEm)}
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                </div>
              )}

              {/* Área de Chat */}
              <div className="flex-1 flex flex-col">
                {/* Mensagens */}
                <div className="flex-1 overflow-y-auto p-4 space-y-4">
                  {loading ? (
                    <div className="text-center py-8 text-muted-foreground">
                      Carregando...
                    </div>
                  ) : questoes.length === 0 ? (
                    <div className="text-center py-8 text-muted-foreground">
                      {MENSAGEM_INICIAL}
                    </div>
                  ) : (
                    questoes.map((questao) => {
                      const isUsuario = questao.tipo === 1;
                      return (
                        <div
                          key={questao.id}
                          className={cn(
                            "flex gap-3",
                            isUsuario && "flex-row-reverse"
                          )}
                        >
                          {!isUsuario && (
                            <Avatar className="h-8 w-8 flex-shrink-0">
                              <AvatarImage src={IAN_AVATAR} alt="Ian" />
                              <AvatarFallback>Ian</AvatarFallback>
                            </Avatar>
                          )}
                          <div className={cn("flex-1 max-w-[80%]", isUsuario && "flex flex-col items-end")}>
                            <div
                              className={cn(
                                "rounded-lg px-4 py-2",
                                isUsuario
                                  ? "bg-primary text-primary-foreground"
                                  : "bg-muted"
                              )}
                            >
                              <p className="text-sm whitespace-pre-wrap">{questao.mensagem}</p>
                            </div>
                            {!isUsuario && questao.id !== 0 && questao.mensagem !== MENSAGEM_INICIAL && (
                              <div className="mt-2 flex items-center gap-2">
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="h-6 w-6"
                                  onClick={() => copiarMensagem(questao.mensagem)}
                                  title="Copiar"
                                >
                                  <Icon name="content_copy" size={12} />
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className={cn(
                                    "h-6 w-6",
                                    questao.feedback === true && "text-green-600"
                                  )}
                                  onClick={() => abrirModalFeedback(questao.id, true)}
                                  title="Útil"
                                >
                                  <Icon name="thumb_up" size={12} />
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className={cn(
                                    "h-6 w-6",
                                    questao.feedback === false && "text-red-600"
                                  )}
                                  onClick={() => abrirModalFeedback(questao.id, false)}
                                  title="Não útil"
                                >
                                  <Icon name="thumb_down" size={12} />
                                </Button>
                              </div>
                            )}
                          </div>
                        </div>
                      );
                    })
                  )}
                  <div ref={messagesEndRef} />
                </div>

                {/* Input */}
                <div className="border-t border-border p-4">
                  <div className="flex gap-2">
                    <Input
                      ref={inputRef}
                      placeholder="Digite sua mensagem..."
                      value={mensagem}
                      onChange={(e) => setMensagem(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter" && !e.shiftKey) {
                          e.preventDefault();
                          enviarMensagem();
                        }
                      }}
                      disabled={enviando}
                      className="flex-1"
                    />
                    <Button
                      onClick={enviarMensagem}
                      disabled={!mensagem.trim() || enviando}
                      size="icon"
                    >
                      <Send className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Modal de Feedback */}
      <Dialog open={feedbackModalOpen} onOpenChange={setFeedbackModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {feedbackTipo === true ? "Feedback Positivo" : "Feedback Negativo"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="feedback-comentario">
                {feedbackTipo === true 
                  ? "O que você achou útil nesta resposta?" 
                  : "O que podemos melhorar nesta resposta?"}
              </Label>
              <Textarea
                id="feedback-comentario"
                placeholder="Digite seu comentário (opcional)..."
                value={feedbackComentario}
                onChange={(e) => setFeedbackComentario(e.target.value)}
                rows={4}
                className="resize-none"
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setFeedbackModalOpen(false);
                setFeedbackComentario("");
              }}
            >
              Cancelar
            </Button>
            <Button onClick={enviarFeedback}>
              Enviar Feedback
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
};
