import { http } from './http'

export function getGroupsApi() {
  return http('/Groups')
}

export function getGroupMembersApi(groupId) {
  return http(`/GroupMembers/${groupId}/members`)
}
