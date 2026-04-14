import type { Config } from "tailwindcss";
import tailwindcssAnimate from "tailwindcss-animate";

const config: Config = {
  darkMode: "class",
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    container: {
      center: true,
      padding: "1.5rem",
      screens: {
        "2xl": "1400px",
      },
    },
    extend: {
      fontFamily: {
        sans: ["Inter", "sans-serif"],
        display: ["Plus Jakarta Sans", "sans-serif"],
        body: ["Inter", "sans-serif"],
      },
      colors: {
        primary: {
          DEFAULT: "var(--color-primary)",
          foreground: "var(--color-text-inverse)",
          soft: "var(--color-primary-soft)",
          strong: "var(--color-primary-strong)",
        },
        primarySoft: "var(--color-primary-soft)",
        primaryStrong: "var(--color-primary-strong)",

        accent: {
          DEFAULT: "var(--color-accent)",
          foreground: "var(--color-text-inverse)",
          soft: "var(--color-accent-soft)",
        },
        accentSoft: "var(--color-accent-soft)",

        primaryText: "var(--color-primary-text)",
        secondaryText: "var(--color-secondary-text)",
        placeholder: "var(--color-text-placeholder)",
        inverseText: "var(--color-text-inverse)",

        primaryBackground: "var(--color-primary-background)",
        secondaryBackground: "var(--color-secondary-background)",
        surfaceElevated: "var(--color-surface-elevated)",
        surfaceSubtle: "var(--color-surface-subtle)",

        borderDefault: "var(--color-border-default)",
        borderSoft: "var(--color-border-soft)",
        field001: "var(--color-field-001)",

        btnPrimary: "var(--color-btn-primary)",
        btnPrimaryHover: "var(--color-btn-primary-hover)",
        btnSecondary: "var(--color-btn-secondary)",
        btnSecondaryText: "var(--color-btn-secondary-text)",
        btnGhostHover: "var(--color-btn-ghost-hover)",

        success: "var(--color-success)",
        warning: "var(--color-warning)",
        error: "var(--color-error)",
        info: "var(--color-info)",

        // Compatibilidade com tokens anteriores
        background: "var(--color-secondary-background)",
        foreground: "var(--color-primary-text)",
        card: {
          DEFAULT: "var(--color-surface-elevated)",
          foreground: "var(--color-primary-text)",
        },
        popover: {
          DEFAULT: "var(--color-surface-elevated)",
          foreground: "var(--color-primary-text)",
        },
        muted: {
          DEFAULT: "var(--color-surface-subtle)",
          foreground: "var(--color-secondary-text)",
        },
        destructive: {
          DEFAULT: "var(--color-error)",
          foreground: "var(--color-text-inverse)",
        },
        secondary: {
          DEFAULT: "var(--color-primary-soft)",
          foreground: "var(--color-primary-text)",
        },
        border: "var(--color-border-default)",
        input: "var(--color-field-001)",
        ring: "var(--color-primary)",
        sidebar: {
          DEFAULT: "var(--color-surface-elevated)",
          foreground: "var(--color-primary-text)",
          primary: "var(--color-primary)",
          "primary-foreground": "var(--color-text-inverse)",
          accent: "var(--color-accent)",
          "accent-foreground": "var(--color-text-inverse)",
          border: "var(--color-border-default)",
          ring: "var(--color-primary)",
        },
      },
      borderRadius: {
        xsToken: "var(--radius-xs)",
        smToken: "var(--radius-sm)",
        mdToken: "var(--radius-md)",
        lgToken: "var(--radius-lg)",
        pillToken: "var(--radius-pill)",
        lg: "var(--radius-lg)",
        md: "var(--radius-md)",
        sm: "var(--radius-sm)",
      },
      spacing: {
        "2xs": "var(--space-2xs)",
        xs: "var(--space-xs)",
        sm: "var(--space-sm)",
        md: "var(--space-md)",
        lg: "var(--space-lg)",
        xl: "var(--space-xl)",
      },
      boxShadow: {
        softToken: "var(--elevation-soft)",
        cardHoverToken: "var(--elevation-card-hover)",
      },
      backgroundImage: {
        "brand-gradient": "var(--color-background-brand)",
      },
      keyframes: {
        "accordion-down": {
          from: { height: "0" },
          to: { height: "var(--radix-accordion-content-height)" },
        },
        "accordion-up": {
          from: { height: "var(--radix-accordion-content-height)" },
          to: { height: "0" },
        },
        "loading-bar": {
          "0%": { transform: "translateX(-100%)" },
          "100%": { transform: "translateX(400%)" },
        },
        "loading-fill": {
          "0%": { width: "0%" },
          "100%": { width: "100%" },
        },
      },
      animation: {
        "accordion-down": "accordion-down 0.2s ease-out",
        "accordion-up": "accordion-up 0.2s ease-out",
        "loading-bar": "loading-bar 1.5s ease-in-out infinite",
        "loading-fill": "loading-fill 1.2s ease-in-out infinite",
      },
    },
  },
  plugins: [tailwindcssAnimate],
};

export default config;
