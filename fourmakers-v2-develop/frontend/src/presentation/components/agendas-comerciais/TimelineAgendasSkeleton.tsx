import { Card, CardContent, CardHeader } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'

export function TimelineAgendasSkeleton() {
  return (
    <div className="space-y-8">
      {[1, 2, 3].map((day) => (
        <div key={day}>
          {/* Cabeçalho da Data Skeleton */}
          <div className="flex items-center gap-3 mb-4">
            <div className="flex-1 h-px bg-border" />
            <Skeleton className="h-6 w-48" />
            <div className="flex-1 h-px bg-border" />
          </div>

          {/* Cards Skeleton - Layout em lista vertical (space-y-4) */}
          <div className="space-y-4">
            {[1, 2, 3].map((card) => (
              <Card key={card} className="rounded-lg">
                <CardHeader className="pb-3">
                  <div className="flex items-start justify-between gap-2">
                    <div className="flex-1 space-y-2">
                      <Skeleton className="h-4 w-20" />
                      <Skeleton className="h-5 w-full" />
                    </div>
                    <Skeleton className="h-5 w-16" />
                  </div>
                </CardHeader>
                <CardContent className="space-y-2">
                  <Skeleton className="h-4 w-32" />
                  <Skeleton className="h-4 w-40" />
                  <Skeleton className="h-4 w-28" />
                  <Skeleton className="h-12 w-full mt-2" />
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
