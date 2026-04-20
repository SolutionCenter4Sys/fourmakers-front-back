import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Placeholder from '@tiptap/extension-placeholder';
import { Bold, Italic, List, ListOrdered } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';

/**
 * Converte HTML do editor em texto puro preservando quebras de linha
 * (parágrafos, listas e <br> viram \n para a exibição manter formatação).
 */
export function htmlToPlainText(html: string): string {
  if (!html || html === '<p></p>') return '';
  const div = document.createElement('div');
  div.innerHTML = html;
  // Troca blocos por newline antes de extrair texto, para preservar quebras
  const blockTags = /<\/(p|div|li|h[1-6]|tr)>/gi;
  const br = /<br\s*\/?>/gi;
  let withNewlines = div.innerHTML
    .replace(br, '\n')
    .replace(blockTags, '\n');
  const temp = document.createElement('div');
  temp.innerHTML = withNewlines;
  const text = (temp.innerText || temp.textContent || '').trim();
  return text.replace(/\n{3,}/g, '\n\n'); // no máximo 2 \n seguidos
}

interface PautaSugeridaEditorProps {
  content: string;
  onChange: (html: string) => void;
  placeholder?: string;
  className?: string;
  minHeight?: string;
  'data-testid'?: string;
}

export function PautaSugeridaEditor({
  content,
  onChange,
  placeholder = 'Anote os assuntos que você gostaria de discutir na próxima reunião...',
  className,
  minHeight = 'min-h-[220px]',
  'data-testid': dataTestId,
}: PautaSugeridaEditorProps) {
  const editor = useEditor({
    extensions: [
      StarterKit.configure({
        heading: false,
        codeBlock: false,
        blockquote: false,
        horizontalRule: false,
      }),
      Placeholder.configure({ placeholder }),
    ],
    content,
    onUpdate: ({ editor: ed }) => {
      onChange(ed.getHTML());
    },
    editorProps: {
      attributes: {
        class: cn(
          'prose prose-sm dark:prose-invert max-w-none focus:outline-none px-3 py-3',
          minHeight
        ),
      },
    },
  });

  if (!editor) {
    return null;
  }

  return (
    <div
      className={cn(
        'rounded-lg border-2 border-border bg-background overflow-hidden ring-1 ring-border',
        className
      )}
      data-testid={dataTestId}
    >
      <div className="flex flex-wrap items-center gap-1 p-1.5 border-b border-border bg-muted/30">
        <Button
          type="button"
          variant={editor.isActive('bold') ? 'secondary' : 'ghost'}
          size="sm"
          onClick={() => editor.chain().focus().toggleBold().run()}
          className="h-8 w-8 p-0"
          data-testid={dataTestId ? `${dataTestId}-bold-button` : undefined}
        >
          <Bold className="h-4 w-4" />
        </Button>
        <Button
          type="button"
          variant={editor.isActive('italic') ? 'secondary' : 'ghost'}
          size="sm"
          onClick={() => editor.chain().focus().toggleItalic().run()}
          className="h-8 w-8 p-0"
          data-testid={dataTestId ? `${dataTestId}-italic-button` : undefined}
        >
          <Italic className="h-4 w-4" />
        </Button>
        <Button
          type="button"
          variant={editor.isActive('bulletList') ? 'secondary' : 'ghost'}
          size="sm"
          onClick={() => editor.chain().focus().toggleBulletList().run()}
          className="h-8 w-8 p-0"
          data-testid={dataTestId ? `${dataTestId}-bullet-list-button` : undefined}
        >
          <List className="h-4 w-4" />
        </Button>
        <Button
          type="button"
          variant={editor.isActive('orderedList') ? 'secondary' : 'ghost'}
          size="sm"
          onClick={() => editor.chain().focus().toggleOrderedList().run()}
          className="h-8 w-8 p-0"
          data-testid={dataTestId ? `${dataTestId}-ordered-list-button` : undefined}
        >
          <ListOrdered className="h-4 w-4" />
        </Button>
      </div>
      <div className="min-h-[220px] border-t border-border">
        <EditorContent editor={editor} />
      </div>
    </div>
  );
}
