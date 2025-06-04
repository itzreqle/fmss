"use client"

import type React from "react"
import { createContext, useContext, useEffect, useState } from "react"

const THEME_STORAGE_KEY = "app-theme"
const MODE_STORAGE_KEY = "app-mode"

const THEMES = ["neutral", "stone", "zinc", "gray", "slate", "red", "rose", "blue", "violet"] as const

type Theme = (typeof THEMES)[number]
type Mode = "light" | "dark"

interface ThemeContextProps {
  theme: Theme
  mode: Mode
  setTheme: (theme: Theme) => void
  setMode: (mode: Mode) => void
  toggleMode: () => void
}

const ThemeContext = createContext<ThemeContextProps | undefined>(undefined)

export function useTheme() {
  const context = useContext(ThemeContext)
  if (!context) {
    throw new Error("useTheme must be used within ThemeProvider")
  }
  return context
}

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [theme, setThemeState] = useState<Theme>("neutral")
  const [mode, setModeState] = useState<Mode>("light")
  const [mounted, setMounted] = useState(false)

  // Initialize theme and mode from localStorage
  useEffect(() => {
    const savedTheme = (localStorage.getItem(THEME_STORAGE_KEY) as Theme) || "neutral"
    const savedMode = (localStorage.getItem(MODE_STORAGE_KEY) as Mode) || "light"

    setThemeState(savedTheme)
    setModeState(savedMode)
    setMounted(true)
  }, [])

  // Apply theme and mode to HTML element
  useEffect(() => {
    if (!mounted) return

    const html = document.documentElement

    // Remove all theme classes
    THEMES.forEach((t) => html.classList.remove(`theme-${t}`))

    // Remove dark class
    html.classList.remove("dark")

    // Apply current theme and mode
    html.classList.add(`theme-${theme}`)
    if (mode === "dark") {
      html.classList.add("dark")
    }

    // Save to localStorage
    localStorage.setItem(THEME_STORAGE_KEY, theme)
    localStorage.setItem(MODE_STORAGE_KEY, mode)
  }, [theme, mode, mounted])

  const setTheme = (newTheme: Theme) => {
    setThemeState(newTheme)
  }

  const setMode = (newMode: Mode) => {
    setModeState(newMode)
  }

  const toggleMode = () => {
    setModeState(mode === "light" ? "dark" : "light")
  }

  // Prevent hydration mismatch
  if (!mounted) {
    return null
  }

  return (
    <ThemeContext.Provider value={{ theme, mode, setTheme, setMode, toggleMode }}>{children}</ThemeContext.Provider>
  )
}
