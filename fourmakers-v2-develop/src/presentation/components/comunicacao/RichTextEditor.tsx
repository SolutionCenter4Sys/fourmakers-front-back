import { useMemo, useState } from 'react';
import { container } from 'tsyringe';
import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Placeholder from '@tiptap/extension-placeholder';
import TextAlign from '@tiptap/extension-text-align';
import Underline from '@tiptap/extension-underline';
import Link from '@tiptap/extension-link';
import {
  Bold,
  Italic,
  Underline as UnderlineIcon,
  Strikethrough,
  List,
  ListOrdered,
  Heading1,
  Heading2,
  Heading3,
  AlignLeft,
  AlignCenter,
  AlignRight,
  AlignJustify,
  Link2,
  Quote,
  Undo,
  Redo,
  Code,
  Sparkles,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { Card } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { toast } from 'sonner';
import { useAppSelector } from '@app/store/hooks';
import { ProcessarConteudoIaComunicacaoUseCase } from '@domain/usecases/ProcessarConteudoIaComunicacaoUseCase';
import type { AssistenteIaComunicacaoModo } from '@domain/entities/comunicacao';

/** Ações da UI do menu IA → modo enviado à API */
const MODO_POR_ACAO: Record<string, AssistenteIaComunicacaoModo> = {
  improve: 'melhorar_texto',
  professional: 'mais_profissional',
  summarize: 'resumo',
  executive: 'sumario_executivo',
  topics: 'adicionar_topicos',
  expand: 'expandir_conteudo',
};

/**
 * Se a API devolver texto puro, envolve em parágrafos seguros para o TipTap.
 * Se já vier HTML, repassa sem alterar.
 */
function textoProcessadoParaHtmlEditor(texto: string): string {
  const t = texto.trim();
  if (!t) return '<p></p>';
  if (/<[a-z][\s\S]*>/i.test(t)) {
    return texto;
  }
  const paragrafos = texto.split(/\n+/).map((linha) => {
    const escapado = linha
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
    return escapado.length > 0 ? `<p>${escapado}</p>` : '';
  });
  const html = paragrafos.filter(Boolean).join('');
  return html.length > 0 ? html : '<p></p>';
}

interface RichTextEditorProps {
  content: string;
  onChange: (html: string) => void;
  placeholder?: string;
}

export function RichTextEditor({
  content,
  onChange,
  placeholder = 'Escreva o conteúdo do seu post...',
}: RichTextEditorProps) {
  const [isAIProcessing, setIsAIProcessing] = useState(false);
  const token = useAppSelector((state) => state.auth.token);
  const processarConteudoIa = useMemo(
    () => container.resolve(ProcessarConteudoIaComunicacaoUseCase),
    [],
  );

  const editor = useEditor({
    extensions: [
      StarterKit,
      Underline,
      TextAlign.configure({
        types: ['heading', 'paragraph'],
      }),
      Link.configure({
        openOnClick: false,
      }),
      Placeholder.configure({
        placeholder,
      }),
    ],
    content,
    onUpdate: ({ editor: ed }) => {
      onChange(ed.getHTML());
    },
    editorProps: {
      attributes: {
        class:
          'prose prose-sm dark:prose-invert max-w-none min-h-[200px] focus:outline-none px-4 py-3',
      },
    },
  });

  const addLink = () => {
    if (!editor) return;
    const url = window.prompt('Digite a URL:');
    if (url) {
      editor.chain().focus().setLink({ href: url }).run();
    }
  };

  const handleAIAction = async (action: string) => {
    if (!editor) return;
    const currentText = editor.getText();

    if (!currentText.trim()) {
      toast.error('Conteúdo vazio', {
        description: 'Escreva algum conteúdo primeiro para usar a IA.',
      });
      return;
    }

    const modo = MODO_POR_ACAO[action];
    if (!modo) {
      toast.error('Ação inválida', {
        description: 'Selecione uma opção válida do assistente.',
      });
      return;
    }

    if (!token?.trim()) {
      toast.error('Sessão necessária', {
        description: 'Faça login novamente para usar o assistente de IA.',
      });
      return;
    }

    setIsAIProcessing(true);

    try {
      const resultado = await processarConteudoIa.execute(token, {
        texto: editor.getHTML(),
        modo,
        negrito: false,
      });

      if (!resultado.sucesso) {
        toast.error('Assistente de IA', {
          description: resultado.mensagem,
        });
        return;
      }

      const html = textoProcessadoParaHtmlEditor(resultado.texto);
      editor.commands.setContent(html);
      toast.success('Conteúdo processado!', {
        description: 'O texto foi atualizado conforme a opção escolhida.',
      });
    } catch (error) {
      const mensagem =
        error instanceof Error ? error.message : 'Erro ao processar com a IA.';
      toast.error('Assistente de IA', { description: mensagem });
    } finally {
      setIsAIProcessing(false);
    }
  };

  if (!editor) {
    return null;
  }

  const text = editor.getText();
  const charCount = text.length;
  const wordCount = text.split(/\s+/).filter(Boolean).length;

  return (
    <div className="space-y-2">
      <Card className="border-2 focus-within:border-primary transition-colors">
        {/* Toolbar */}
        <div className="flex flex-wrap items-center gap-1 p-2 border-b bg-muted/30">
          <div className="flex items-center gap-1">
            <Button
              type="button"
              variant={editor.isActive('bold') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleBold().run()}
              className="h-8 w-8 p-0"
            >
              <Bold className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('italic') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleItalic().run()}
              className="h-8 w-8 p-0"
            >
              <Italic className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('underline') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleUnderline().run()}
              className="h-8 w-8 p-0"
            >
              <UnderlineIcon className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('strike') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleStrike().run()}
              className="h-8 w-8 p-0"
            >
              <Strikethrough className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('code') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleCode().run()}
              className="h-8 w-8 p-0"
            >
              <Code className="h-4 w-4" />
            </Button>
          </div>

          <Separator orientation="vertical" className="h-6" />

          <div className="flex items-center gap-1">
            <Button
              type="button"
              variant={editor.isActive('heading', { level: 1 }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleHeading({ level: 1 }).run()}
              className="h-8 w-8 p-0"
            >
              <Heading1 className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('heading', { level: 2 }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
              className="h-8 w-8 p-0"
            >
              <Heading2 className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('heading', { level: 3 }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}
              className="h-8 w-8 p-0"
            >
              <Heading3 className="h-4 w-4" />
            </Button>
          </div>

          <Separator orientation="vertical" className="h-6" />

          <div className="flex items-center gap-1">
            <Button
              type="button"
              variant={editor.isActive('bulletList') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleBulletList().run()}
              className="h-8 w-8 p-0"
            >
              <List className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('orderedList') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleOrderedList().run()}
              className="h-8 w-8 p-0"
            >
              <ListOrdered className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive('blockquote') ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().toggleBlockquote().run()}
              className="h-8 w-8 p-0"
            >
              <Quote className="h-4 w-4" />
            </Button>
          </div>

          <Separator orientation="vertical" className="h-6" />

          <div className="flex items-center gap-1">
            <Button
              type="button"
              variant={editor.isActive({ textAlign: 'left' }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().setTextAlign('left').run()}
              className="h-8 w-8 p-0"
            >
              <AlignLeft className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive({ textAlign: 'center' }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().setTextAlign('center').run()}
              className="h-8 w-8 p-0"
            >
              <AlignCenter className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive({ textAlign: 'right' }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().setTextAlign('right').run()}
              className="h-8 w-8 p-0"
            >
              <AlignRight className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant={editor.isActive({ textAlign: 'justify' }) ? 'secondary' : 'ghost'}
              size="sm"
              onClick={() => editor.chain().focus().setTextAlign('justify').run()}
              className="h-8 w-8 p-0"
            >
              <AlignJustify className="h-4 w-4" />
            </Button>
          </div>

          <Separator orientation="vertical" className="h-6" />

          <Button
            type="button"
            variant={editor.isActive('link') ? 'secondary' : 'ghost'}
            size="sm"
            onClick={addLink}
            className="h-8 w-8 p-0"
          >
            <Link2 className="h-4 w-4" />
          </Button>

          <Separator orientation="vertical" className="h-6" />

          <div className="flex items-center gap-1">
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => editor.chain().focus().undo().run()}
              disabled={!editor.can().undo()}
              className="h-8 w-8 p-0"
            >
              <Undo className="h-4 w-4" />
            </Button>
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => editor.chain().focus().redo().run()}
              disabled={!editor.can().redo()}
              className="h-8 w-8 p-0"
            >
              <Redo className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1" />

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                type="button"
                variant="primary"
                size="sm"
                className="gap-2"
                disabled={isAIProcessing}
              >
                <Sparkles className="h-4 w-4" />
                <span className="hidden sm:inline">
                  {isAIProcessing ? 'Processando...' : 'IA'}
                </span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <div className="px-2 py-1.5 text-sm font-semibold">
                Assistente de Conteúdo ✨
              </div>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => handleAIAction('improve')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Melhorar texto
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleAIAction('professional')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Tornar mais profissional
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleAIAction('summarize')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Criar resumo
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => handleAIAction('executive')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Gerar sumário executivo
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleAIAction('topics')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Adicionar tópicos
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => handleAIAction('expand')}>
                <Sparkles className="mr-2 h-4 w-4" />
                Expandir conteúdo
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        <div className="relative">
          <EditorContent editor={editor} />
          {isAIProcessing && (
            <div className="absolute inset-0 bg-background/80 backdrop-blur-sm flex items-center justify-center rounded-b-lg">
              <div className="flex items-center gap-3 text-primary">
                <Sparkles className="w-6 h-6 animate-pulse" />
                <span className="font-medium">IA processando seu conteúdo...</span>
              </div>
            </div>
          )}
        </div>

        <div className="px-4 py-2 border-t bg-muted/30 flex items-center justify-between text-xs text-muted-foreground">
          <span>{charCount} caracteres</span>
          <span>{wordCount} palavras</span>
        </div>
      </Card>

      <p className="text-xs text-muted-foreground">
        💡 Use o botão <strong>IA</strong> para melhorar automaticamente seu conteúdo
      </p>
    </div>
  );
}
