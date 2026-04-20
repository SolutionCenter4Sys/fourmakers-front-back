import { Skeleton } from "@/components/ui/skeleton";

export const EducationSectionSkeleton = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Skeleton className="h-5 w-5" />
          <Skeleton className="h-7 w-32" />
        </div>
        <Skeleton className="h-9 w-40" />
      </div>

      <div className="flex items-center justify-between pt-2">
        <Skeleton className="h-6 w-40" />
      </div>

      <div className="space-y-3">
        {[1, 2].map((i) => (
          <div key={i} className="rounded-xl border bg-card p-4">
            <div className="flex items-start gap-4">
              <Skeleton className="h-10 w-10 rounded-lg" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-5 w-64" />
                <Skeleton className="h-4 w-48" />
                <div className="flex gap-3 mt-2">
                  <Skeleton className="h-6 w-24 rounded-full" />
                  <Skeleton className="h-4 w-32" />
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
