import { Outlet, createRootRouteWithContext } from '@tanstack/react-router'
import type { AuthContext } from '@/lib/state/auth'

interface RouterContext {
  auth: AuthContext
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
})

function RootComponent() {
  return (
    <>
      <Outlet />
    </>
  )
}
