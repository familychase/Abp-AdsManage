import { i18n } from '@/plugins/vueI18n'
import { useLocaleStoreWithOut } from '@/store/modules/locale'
import { setHtmlPageLang } from '@/plugins/vueI18n/helper'
import { useUserStoreWithOut } from '@/store/modules/user'
import { usePermissionStoreWithOut } from '@/store/modules/permission'
import { getSelfMenuApi, convertMenusToRoutes } from '@/api/login'

const setI18nLanguage = (locale: LocaleType) => {
  const localeStore = useLocaleStoreWithOut()

  if (i18n.mode === 'legacy') {
    i18n.global.locale = locale
  } else {
    ;(i18n.global.locale as any).value = locale
  }
  localeStore.setCurrentLocale({
    lang: locale
  })
  setHtmlPageLang(locale)
}

export const useLocale = () => {
  const changeLocale = async (locale: LocaleType) => {
    const globalI18n = i18n.global

    const langModule = await import(`../../locales/${locale}.ts`)
    globalI18n.setLocaleMessage(locale, langModule.default)

    setI18nLanguage(locale)

    // 切换语言后重新获取服务端菜单，使路由 meta.title 同步语言
    try {
      const userStore = useUserStoreWithOut()
      const permissionStore = usePermissionStoreWithOut()
      if (userStore.getToken) {
        const res = await getSelfMenuApi()
        if (res?.data) {
          const menus = res.data.menus || []
          const routers = convertMenusToRoutes(menus)
          userStore.setRoleRouters(routers)
          await permissionStore.generateRoutes('server', routers).catch(() => {})
        }
      }
    } catch {
      // 菜单刷新失败不影响语言切换
    }
  }

  return {
    changeLocale
  }
}
