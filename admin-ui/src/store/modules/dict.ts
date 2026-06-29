import { defineStore } from 'pinia'
import { store } from '../index'
import { getDictItemWebListApi } from '@/api/system/dict'
import type { DictItemDto } from '@/api/system/dict/types'

/** 按 dictSortCode 分组的字典项 */
type DictCache = Record<string, DictItemDto[]>

interface DictState {
  /** 按 dictSortCode 分组的缓存 */
  cache: DictCache
  loaded: boolean
  loading: boolean
}

export const useDictStore = defineStore('dict', {
  state: (): DictState => ({
    cache: {},
    loaded: false,
    loading: false
  }),
  getters: {
    /** 获取指定 sortCode 下的字典项列表 */
    getItems(): (sortCode: string) => DictItemDto[] {
      return (sortCode: string) => this.cache[sortCode] ?? []
    },
    /** 根据 sortCode + itemCode 取 label（中文名） */
    getLabel(): (sortCode: string, itemCode: string) => string {
      return (sortCode: string, itemCode: string) => {
        const items = this.cache[sortCode] ?? []
        return items.find((i) => i.dictItemCode === itemCode)?.dictItemName ?? itemCode
      }
    },
    /** 获取指定 sortCode 下的下拉选项 */
    getOptions(): (sortCode: string) => { label: string; value: string }[] {
      return (sortCode: string) =>
        (this.cache[sortCode] ?? []).map((item) => ({
          label: item.dictItemName ?? item.dictItemCode ?? '',
          value: item.dictItemCode ?? ''
        }))
    },
    /** 是否已加载 */
    isLoaded(): boolean {
      return this.loaded
    }
  },
  actions: {
    async fetchWebList(sortCodes: string[] = [], force = false) {
      if ((this.loaded && !force) || this.loading) {
        return
      }
      this.loading = true
      try {
        const res = await getDictItemWebListApi(sortCodes)
        if (res?.data) {
          this.cache = { ...this.cache, ...res.data }
          this.loaded = true
        }
      } catch {
        // 静默失败
      } finally {
        this.loading = false
      }
    },
    clear() {
      this.cache = {}
      this.loaded = false
    }
  },
  persist: true
})

export const useDictStoreWithOut = () => useDictStore(store)
