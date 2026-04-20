
export interface AnalyticsEvent {
  action?: string;
  category?: string; 
  section?: string;
  label?: string;
  value?: number;
  [key: string]: any;
}

export const trackEvent = (eventName: string, params: AnalyticsEvent) => {
  const eventData = {
    event: eventName,
    timestamp: new Date().toISOString(),
    ...params
  };

  console.log(`[Analytics 📊] ${eventName}:`, eventData);
  
  if (typeof window !== 'undefined' && (window as any).gtag) {
    (window as any).gtag('event', eventName, params);
  }
};
