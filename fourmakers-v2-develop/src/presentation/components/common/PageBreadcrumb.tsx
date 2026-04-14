import { Link } from 'react-router-dom'
import {
  Breadcrumb,
  BreadcrumbList,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'

export interface BreadcrumbItem {
  label: string
  /** Caminho para navegação. Quando informado junto com `state`, o Link usa `to={{ pathname: href, state }}`. */
  href?: string
  /** State para passar na navegação (ex.: voltar para Candidatos com vagaId/vagaTitle). */
  state?: Record<string, unknown>
}

interface PageBreadcrumbProps {
  items: BreadcrumbItem[]
}

export const PageBreadcrumb = ({ items }: PageBreadcrumbProps) => {
  return (
    <Breadcrumb className="mb-4">
      <BreadcrumbList>
        <BreadcrumbItem>
          <BreadcrumbLink asChild>
            <Link to="/dashboard">
              Início
            </Link>
          </BreadcrumbLink>
        </BreadcrumbItem>
        
        {items.map((item, index) => {
          const isLast = index === items.length - 1
          
          return (
            <div key={index} className="flex items-center gap-1.5">
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                {isLast || !item.href ? (
                  <BreadcrumbPage>{item.label}</BreadcrumbPage>
                ) : (
                  <BreadcrumbLink asChild>
                    <Link to={item.href} {...(item.state != null && { state: item.state })}>
                      {item.label}
                    </Link>
                  </BreadcrumbLink>
                )}
              </BreadcrumbItem>
            </div>
          )
        })}
      </BreadcrumbList>
    </Breadcrumb>
  )
}

