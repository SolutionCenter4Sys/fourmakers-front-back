import * as React from "react";
import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";

const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-pillToken px-md py-xs text-sm font-semibold tracking-tight ring-offset-secondaryBackground transition duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-60 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0 [&_.material-symbols-outlined]:pointer-events-none [&_.material-symbols-outlined]:shrink-0 [&_.material-symbols-outlined]:text-[16px] [&_.material-symbols-outlined]:leading-none",
  {
    variants: {
      variant: {
        primary: "bg-btnPrimary text-inverseText shadow-softToken hover:bg-btnPrimaryHover",
        secondary: "bg-btnSecondary text-btnSecondaryText border border-borderDefault hover:bg-primarySoft",
        ghost: "bg-transparent text-primary hover:bg-btnGhostHover dark:hover:bg-white/5",
        outline: "border border-borderDefault text-primary hover:bg-primarySoft",
        destructive: "bg-error text-inverseText hover:bg-error/90",
        link: "text-primary underline-offset-4 hover:underline",
      },
      size: {
        default: "h-11 px-lg py-sm",
        sm: "h-9 px-sm py-2xs text-xs",
        lg: "h-12 px-xl py-sm text-base",
        icon: "h-11 w-11 p-0 rounded-pillToken",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "default",
    },
  },
);

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean;
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : "button";
    return <Comp className={cn(buttonVariants({ variant, size, className }))} ref={ref} {...props} />;
  },
);
Button.displayName = "Button";

export { Button, buttonVariants };
