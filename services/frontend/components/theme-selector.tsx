"use client"

import { Moon, Sun } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { useTheme } from "./theme-provider"

const THEMES = [
  { value: "neutral", label: "Neutral" },
  { value: "stone", label: "Stone" },
  { value: "zinc", label: "Zinc" },
  { value: "gray", label: "Gray" },
  { value: "slate", label: "Slate" },
  { value: "red", label: "Red" },
  { value: "rose", label: "Rose" },
  { value: "blue", label: "Blue" },
  { value: "violet", label: "Violet" },
] as const

export function ThemeSelector() {
  const { theme, mode, setTheme, toggleMode } = useTheme()

  return (
    <div className="flex items-center gap-2">
      <Select value={theme} onValueChange={setTheme}>
        <SelectTrigger aria-label="Select theme" className="w-28">
          <SelectValue placeholder="Theme" />
        </SelectTrigger>
        <SelectContent>
          {THEMES.map((t) => (
            <SelectItem key={t.value} value={t.value}>
              {t.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      <Button
        variant="outline"
        size="icon"
        onClick={toggleMode}
        aria-label={`Switch to ${mode === "light" ? "dark" : "light"} mode`}
      >
        {mode === "light" ? <Moon className="h-4 w-4" /> : <Sun className="h-4 w-4" />}
      </Button>
    </div>
  )
}
