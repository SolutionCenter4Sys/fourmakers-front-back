import { Skeleton } from "@/components/ui/skeleton";

export const CertificationSectionSkeleton = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-5" />
          <Skeleton className="h-7 w-36" />
        </div>
      </div>

      <div className="space-y-3">
        {[1, 2].map((i) => (
          <div key={i} className="rounded-xl border bg-card p-4">
            <div className="flex items-start gap-4">
              <Skeleton className="h-10 w-10 rounded-lg" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-5 w-56" />
                <Skeleton className="h-4 w-40" />
                <div className="flex gap-3 mt-2">
                  <Skeleton className="h-4 w-24" />
                  <Skeleton className="h-4 w-16" />
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
