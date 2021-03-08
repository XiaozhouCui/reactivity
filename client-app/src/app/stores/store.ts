import { createContext, useContext } from 'react'
import ActivityStore from './activityStore'
import CommonStore from './commonStore'

interface Store {
  // ActivityStore CLASS can be used as TYPE
  activityStore: ActivityStore
  commonStore: CommonStore
}

// store will be used in React context provider HOC
export const store: Store = {
  activityStore: new ActivityStore(),
  commonStore: new CommonStore(),
}

export const StoreContext = createContext(store)

// React hook useContext
export function useStore() {
  return useContext(StoreContext)
}
