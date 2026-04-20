import { useAppSelector } from '@app/store/hooks'
import type { WelcomeContent } from '@shared/types/homeBuilder'

interface WelcomeBlockProps {
  content: WelcomeContent
}

export function WelcomeBlock({ content }: WelcomeBlockProps) {
  const { user } = useAppSelector((state) => state.auth)

  const greeting = content.showUserName
    ? content.greeting.replace('{nomeColaborador}', user?.nomeColaborador ?? 'Usuário')
    : content.greeting.replace(', {nomeColaborador}', '').replace('{nomeColaborador}', '')

  return (
    <div className="w-full min-w-0 flex-1 flex flex-col justify-center gap-1 py-2">
      <h2 className="text-xl sm:text-2xl font-bold break-words">{greeting}</h2>
      <p className="text-sm sm:text-base text-secondaryText break-words">{content.subtitle}</p>
    </div>
  )
}
