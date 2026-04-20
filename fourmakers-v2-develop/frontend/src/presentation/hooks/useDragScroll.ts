import { useCallback, useRef, type RefObject } from 'react'

/**
 * Custom hook that enables drag-to-scroll functionality on a scrollable element.
 * Uses a callback ref to ensure event listeners are always properly attached,
 * even when the element is replaced or re-rendered.
 * 
 * @returns A ref to attach to the scrollable element
 * 
 * @example
 * ```tsx
 * const scrollRef = useDragScroll<HTMLDivElement>()
 * return <div ref={scrollRef} className="overflow-auto">...</div>
 * ```
 */
export function useDragScroll<T extends HTMLElement>(): RefObject<T | null> {
  const elementRef = useRef<T | null>(null)
  const isDownRef = useRef(false)
  const startXRef = useRef(0)
  const startYRef = useRef(0)
  const scrollLeftRef = useRef(0)
  const scrollTopRef = useRef(0)
  const cleanupRef = useRef<(() => void) | null>(null)

  const attachListeners = useCallback((element: T) => {
    // Clean up any existing listeners first
    if (cleanupRef.current) {
      cleanupRef.current()
      cleanupRef.current = null
    }

    const handleMouseDown = (e: MouseEvent) => {
      // Ignore if clicking on interactive elements
      const target = e.target as HTMLElement
      if (
        target.tagName === 'BUTTON' ||
        target.tagName === 'A' ||
        target.tagName === 'INPUT' ||
        target.tagName === 'SELECT' ||
        target.tagName === 'TH' ||
        target.closest('button') ||
        target.closest('a') ||
        target.closest('select') ||
        target.closest('th')
      ) {
        return
      }

      isDownRef.current = true
      element.style.cursor = 'grabbing'
      element.style.userSelect = 'none'
      
      startXRef.current = e.pageX - element.offsetLeft
      startYRef.current = e.pageY - element.offsetTop
      scrollLeftRef.current = element.scrollLeft
      scrollTopRef.current = element.scrollTop
    }

    const handleMouseLeave = () => {
      if (isDownRef.current) {
        isDownRef.current = false
        element.style.cursor = 'grab'
        element.style.userSelect = ''
      }
    }

    const handleMouseUp = () => {
      isDownRef.current = false
      element.style.cursor = 'grab'
      element.style.userSelect = ''
    }

    const handleMouseMove = (e: MouseEvent) => {
      if (!isDownRef.current) return
      
      e.preventDefault()
      
      const x = e.pageX - element.offsetLeft
      const y = e.pageY - element.offsetTop
      const walkX = (x - startXRef.current) * 1.5 // Multiplier for scroll speed
      const walkY = (y - startYRef.current) * 1.5
      
      element.scrollLeft = scrollLeftRef.current - walkX
      element.scrollTop = scrollTopRef.current - walkY
    }

    // Set initial cursor style
    element.style.cursor = 'grab'

    // Attach event listeners
    element.addEventListener('mousedown', handleMouseDown)
    element.addEventListener('mouseleave', handleMouseLeave)
    element.addEventListener('mouseup', handleMouseUp)
    element.addEventListener('mousemove', handleMouseMove)

    // Store cleanup function
    cleanupRef.current = () => {
      element.removeEventListener('mousedown', handleMouseDown)
      element.removeEventListener('mouseleave', handleMouseLeave)
      element.removeEventListener('mouseup', handleMouseUp)
      element.removeEventListener('mousemove', handleMouseMove)
      element.style.cursor = ''
      element.style.userSelect = ''
      isDownRef.current = false
    }
  }, [])

  // Callback ref that attaches/detaches listeners when element changes
  const callbackRef = useCallback((element: T | null) => {
    // Clean up previous element
    if (cleanupRef.current) {
      cleanupRef.current()
      cleanupRef.current = null
    }

    // Store and setup new element
    elementRef.current = element
    if (element) {
      attachListeners(element)
    }
  }, [attachListeners])

  // Return as RefObject for compatibility, but it's actually a callback ref
  return callbackRef as unknown as RefObject<T | null>
}
