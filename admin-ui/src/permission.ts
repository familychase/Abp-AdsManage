import router from './router'
import { useAppStoreWithOut } from '@/store/modules/app'
import type { RouteRecordRaw } from 'vue-router'
import { useTitle } from '@/hooks/web/useTitle'
import { useNProgress } from '@/hooks/web/useNProgress'
import { usePermissionStoreWithOut } from '@/store/modules/permission'
import { usePageLoading } from '@/hooks/web/usePageLoading'
import { NO_REDIRECT_WHITE_LIST } from '@/constants'
import { useUserStoreWithOut } from '@/store/modules/user'
import { getSelfMenuApi, convertMenusToRoutes } from '@/api/login'

const { start, done } = useNProgress()

const { loadStart, loadDone } = usePageLoading()

// 是否需要登录验证
const LOGIN_REQUIRED = import.meta.env.VITE_LOGIN_REQUIRED !== 'false'

/** 用 sessionStorage 标记当前会话是否已加载，从而检测强刷（F5） */
const SESSION_KEY = '__app_session_loaded'
const isHardRefresh = sessionStorage.getItem(SESSION_KEY) !== null
sessionStorage.setItem(SESSION_KEY, '1')

router.beforeEach(async (to, from, next) => {
  start()
  loadStart()
  const permissionStore = usePermissionStoreWithOut()
  const appStore = useAppStoreWithOut()
  const userStore = useUserStoreWithOut()

  // 免登录模式：自动设置模拟用户
  if (!LOGIN_REQUIRED && !userStore.getUserInfo) {
    userStore.setUserInfo({
      userCode: 'admin',
      username: '',
      password: '',
      role: 'admin',
      roleId: '1'
    })
    userStore.setToken('')
  }

  // 需要登录模式：通过 token 判断是否已认证
  const isAuthenticated = LOGIN_REQUIRED ? !!userStore.getToken : !!userStore.getUserInfo

  if (isAuthenticated) {
    if (to.path === '/login') {
      next({ path: '/' })
    } else {
      if (permissionStore.getIsAddRouters) {
        next()
        return
      }

      // 强刷时重新拉取服务端菜单，确保侧边栏与后端同步
      if (isHardRefresh && appStore.getDynamicRouter && appStore.getServerDynamicRouter) {
        try {
          const res = await getSelfMenuApi()
          if (res?.data) {
            const menus = res.data.menus || []
            const routers = convertMenusToRoutes(menus)
            userStore.setRoleRouters(routers)
          }
        } catch {
          // 拉取菜单失败则继续使用缓存
        }
      }

      const freshRoleRouters = userStore.getRoleRouters || []

      // 免登录模式使用静态路由（加载所有页面），登录模式按角色动态过滤
      if (!LOGIN_REQUIRED) {
        await permissionStore.generateRoutes('static')
      } else if (appStore.getDynamicRouter) {
        appStore.serverDynamicRouter
          ? await permissionStore.generateRoutes(
              'server',
              freshRoleRouters as AppCustomRouteRecordRaw[]
            )
          : await permissionStore.generateRoutes('frontEnd', freshRoleRouters as string[])
      } else {
        await permissionStore.generateRoutes('static')
      }

      permissionStore.getAddRouters.forEach((route) => {
        router.addRoute(route as unknown as RouteRecordRaw) // 动态添加可访问路由表
      })
      const redirectPath = from.query.redirect || to.path
      const redirect = decodeURIComponent(redirectPath as string)
      const nextData = to.path === redirect ? { ...to, replace: true } : { path: redirect }
      permissionStore.setIsAddRouters(true)
      next(nextData)
    }
  } else {
    if (!LOGIN_REQUIRED || NO_REDIRECT_WHITE_LIST.indexOf(to.path) !== -1) {
      next()
    } else {
      next(`/login?redirect=${to.path}`) // 否则全部重定向到登录页
    }
  }
})

router.afterEach((to) => {
  useTitle(to?.meta?.title as string)
  done() // 结束Progress
  loadDone()
})
