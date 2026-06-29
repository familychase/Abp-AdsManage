import { defineStore } from 'pinia'
import { store } from '../index'
import { UserType } from '@/api/login/types'
import { ElMessageBox } from 'element-plus'
import { useI18n } from '@/hooks/web/useI18n'
import { loginOutApi } from '@/api/login'
import { useTagsViewStore } from './tagsView'
import { usePermissionStore } from './permission'

interface UserState {
  userInfo?: UserType
  token: string
  roleRouters?: string[] | AppCustomRouteRecordRaw[]
  rememberMe: boolean
  loginInfo?: string
}

export const useUserStore = defineStore('user', {
  state: (): UserState => {
    return {
      userInfo: undefined,
      token: '',
      roleRouters: undefined,
      // 记住我
      rememberMe: true,
      loginInfo: undefined
    }
  },
  getters: {
    getToken(): string {
      return this.token
    },
    getUserInfo(): UserType | undefined {
      return this.userInfo
    },
    getRoleRouters(): string[] | AppCustomRouteRecordRaw[] | undefined {
      return this.roleRouters
    },
    getRememberMe(): boolean {
      return this.rememberMe
    },
    getLoginInfo(): string | undefined {
      return this.loginInfo
    }
  },
  actions: {
    setToken(token: string) {
      this.token = token
    },
    setUserInfo(userInfo?: UserType) {
      this.userInfo = userInfo
    },
    setRoleRouters(roleRouters: string[] | AppCustomRouteRecordRaw[]) {
      this.roleRouters = roleRouters
    },
    logoutConfirm() {
      const { t } = useI18n()
      ElMessageBox.confirm(t('common.loginOutMessage'), t('common.reminder'), {
        confirmButtonText: t('common.ok'),
        cancelButtonText: t('common.cancel'),
        type: 'warning'
      })
        .then(async () => {
          // 先尝试调用后端退出接口（失败不影响本地清理）
          await loginOutApi().catch(() => {})
          this.reset()
        })
        .catch(() => {})
    },
    reset() {
      const tagsViewStore = useTagsViewStore()
      const permissionStore = usePermissionStore()
      // 清理标签页
      tagsViewStore.delAllViews()
      // 重置权限路由
      permissionStore.setIsAddRouters(false)
      permissionStore.$reset()
      // 手动清空 user 字段（不用 $reset，避免 persist 插件拦截恢复旧数据）
      this.token = ''
      this.userInfo = undefined
      this.roleRouters = undefined
      this.loginInfo = undefined
      this.rememberMe = false
      // 直接删 localStorage，再整页跳转（新页面 Pinia 从空缓存初始化，无 token）
      localStorage.removeItem('user')
      window.location.replace('/#/login')
    },
    logout() {
      this.reset()
    },
    setRememberMe(rememberMe: boolean) {
      this.rememberMe = rememberMe
    },
    setLoginInfo(loginInfo: string | undefined) {
      this.loginInfo = loginInfo
    }
  },
  persist: true
})

export const useUserStoreWithOut = () => {
  return useUserStore(store)
}
