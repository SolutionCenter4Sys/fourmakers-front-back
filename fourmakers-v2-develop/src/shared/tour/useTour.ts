import { useState, useCallback } from 'react';
import type { CallBackProps, Status } from 'react-joyride';
import { STATUS } from 'react-joyride';

export interface UseTourReturn {
  isRunning: boolean;
  startTour: () => void;
  stopTour: () => void;
  handleJoyrideCallback: (data: CallBackProps) => void;
}

export function useTour(): UseTourReturn {
  const [isRunning, setIsRunning] = useState(false);

  const startTour = useCallback(() => setIsRunning(true), []);
  const stopTour = useCallback(() => setIsRunning(false), []);

  const handleJoyrideCallback = useCallback((data: CallBackProps) => {
    const { status } = data as { status: Status };
    if (status === STATUS.FINISHED || status === STATUS.SKIPPED) {
      setIsRunning(false);
    }
  }, []);

  return { isRunning, startTour, stopTour, handleJoyrideCallback };
}
