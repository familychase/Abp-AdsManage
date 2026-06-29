import { watch, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { isString } from '@/utils/is'
import { useAppStoreWithOut } from '@/store/modules/app'
import { useI18n } from '@/hooks/web/useI18n'
import { i18n } from '@/plugins/vueI18n'

export const useTitle = (newTitle?: string | Ref<string | undefined>) => {
  const { t } = useI18n()
  const appStore = useAppStoreWithOut()

  const resolveTitle = () => {
    const resolvedTitle = unref(newTitle)
    return resolvedTitle ? `${appStore.getTitle} - ${t(resolvedTitle)}` : appStore.getTitle
  }

  const title = ref(resolveTitle())

  // 监听当前路由标题变化
  watch(
    () => unref(newTitle),
    () => {
      title.value = resolveTitle()
    }
  )

  // 监听语言切换
  if (i18n) {
    watch(
      () => (i18n.global.locale as any).value,
      () => {
        title.value = resolveTitle()
      }
    )
  }

  watch(
    title,
    (n, o) => {
      if (isString(n) && n !== o && document) {
        document.title = n
      }
    },
    { immediate: true }
  )

  return title
}
