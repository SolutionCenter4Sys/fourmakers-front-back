import { Skeleton } from "@/components/ui/skeleton";

export const DocumentsSectionSkeleton = () => {
  return (
    <div className="space-y-5">
      <Skeleton className="h-7 w-32" />

      {/* Vistos */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <Skeleton className="h-5 w-20" />
        </div>
        <div className="space-y-2">
          {[1].map((i) => (
            <div key={i} className="flex items-center justify-between rounded-lg border bg-card p-3">
              <div className="flex items-center gap-3">
                <Skeleton className="h-10 w-10 rounded-lg" />
                <div className="space-y-1">
                  <Skeleton className="h-4 w-32" />
                  <Skeleton className="h-3 w-40" />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Passaportes */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <Skeleton className="h-5 w-28" />
        </div>
        <div className="space-y-2">
          {[1].map((i) => (
            <div key={i} className="flex items-center justify-between rounded-lg border bg-card p-3">
              <div className="flex items-center gap-3">
                <Skeleton className="h-10 w-10 rounded-lg" />
                <div className="space-y-1">
                  <Skeleton className="h-4 w-32" />
                  <Skeleton className="h-3 w-40" />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
