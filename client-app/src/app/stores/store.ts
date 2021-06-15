import { createContext, useContext } from 'react'
import ActivityStore from './activityStore'
import CommentStore from './commentStore'
import CommonStore from './commonStore'
import ModalStore from './modalStore'
import ProfileStore from './profileStore'
import UserStore from './userStore'

interface Store {
  // ActivityStore CLASS can be used as TYPE
  activityStore: ActivityStore
  commonStore: CommonStore
  userStore: UserStore
  modalStore: ModalStore
  profileStore: ProfileStore
  commentStore: CommentStore
}

// store will be used in React context provider HOC
export const store: Store = {
  activityStore: new ActivityStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
  profileStore: new ProfileStore(),
  commentStore: new CommentStore(),
}

export const StoreContext = createContext(store)

// React hook useContext
export function useStore() {
  return useContext(StoreContext)
}
