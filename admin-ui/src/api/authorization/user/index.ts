//import apiService from '@/axios/api-service'
import request from '@/axios'
import type {
  SysUserDto,
  SysUserDtoPagedResult,
  GetSysUserListInput,
  CreateUpdateSysUserDto
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

// 获取用户列表（分页）
export const getUserListApi = (data: GetSysUserListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<SysUserDtoPagedResult>({
    url: '/api/system/user/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

// 获取单个用户
export const getUserByIdApi = (id: number) => {
  const userStore = useUserStoreWithOut()
  return request.get<SysUserDto>({
    url: `/api/system/user/${id}`,
    headers: {
      access_token: userStore.getToken
    }
  })
}

// 创建用户
export const createUserApi = (data: CreateUpdateSysUserDto) => {
  const userStore = useUserStoreWithOut()
  return request.post<SysUserDto>({
    url: '/api/system/user',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

// 更新用户
export const updateUserApi = (id: number, data: CreateUpdateSysUserDto) => {
  const userStore = useUserStoreWithOut()
  return request.put<SysUserDto>({
    url: `/api/system/user/${id}`,
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

// 重置用户密码
export const resetUserPasswordApi = (id: number) => {
  const userStore = useUserStoreWithOut()
  return request.post<void>({
    url: `/api/system/user/${id}/reset_pswd`,
    headers: {
      access_token: userStore.getToken
    }
  })
}

// 删除用户
export const deleteUserApi = (id: number) => {
  const userStore = useUserStoreWithOut()
  return request.delete({
    url: `/api/system/user/${id}`,
    headers: {
      access_token: userStore.getToken
    }
  })
}
