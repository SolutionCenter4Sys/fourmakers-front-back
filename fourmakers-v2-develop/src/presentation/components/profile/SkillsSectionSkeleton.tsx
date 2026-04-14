import { Skeleton } from "@/components/ui/skeleton";

export const SkillsSectionSkeleton = () => {
  return (
    <div className="space-y-5">
      <Skeleton className="h-7 w-32" />
      
      <div className="space-y-5">
        {[1, 2, 3].map((category) => (
          <div key={category} className="space-y-2.5">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Skeleton className="h-4 w-4" />
                <Skeleton className="h-5 w-28" />
              </div>
            </div>
            
            <div className="flex flex-wrap gap-2">
              {[1, 2, 3, 4].map((skill) => (
                <Skeleton key={skill} className="h-7 w-24 rounded-full" />
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
