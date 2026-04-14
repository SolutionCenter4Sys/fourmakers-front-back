import * as React from "react";
// @radix-ui/react-slider não está instalado - componente temporário
import { cn } from "@/lib/utils";

interface SliderProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'type' | 'value' | 'defaultValue' | 'onChange'> {
  value?: number[];
  defaultValue?: number[];
  onValueChange?: (value: number[]) => void;
}

const Slider = React.forwardRef<HTMLInputElement, SliderProps>(
  ({ className, value, defaultValue, onValueChange, ...props }, ref) => {
    const [internalValue, setInternalValue] = React.useState<number[]>(
      value || defaultValue || [0]
    );

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const newValue = [Number(e.target.value)];
      setInternalValue(newValue);
      onValueChange?.(newValue);
    };

    const currentValue = value ? value[0] : (internalValue[0] || 0);

    return (
      <div className={cn("relative flex w-full touch-none select-none items-center", className)}>
        <input
          ref={ref}
          type="range"
          value={currentValue}
          onChange={handleChange}
          className="w-full h-2 bg-secondary rounded-full appearance-none cursor-pointer"
          {...props}
        />
      </div>
    );
  }
);
Slider.displayName = "Slider";

export { Slider };
