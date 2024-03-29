import { makeAutoObservable, runInAction } from 'mobx'
import { history } from '../..'
import agent from '../api/agent'
import { User, UserFormValues } from '../models/user'
import { store } from './store'

export default class UserStore {
  user: User | null = null

  constructor() {
    makeAutoObservable(this)
  }

  // computed property to check if user is logged in
  get isLoggedIn() {
    return !!this.user
  }

  login = async (creds: UserFormValues) => {
    try {
      const user = await agent.Account.login(creds)
      // save the token in common store
      store.commonStore.setToken(user.token)
      // observable can only be modified inside an action
      runInAction(() => (this.user = user))
      history.push('/activities')
      store.modalStore.closeModal()
    } catch (error) {
      throw error
    }
  }

  logout = () => {
    store.commonStore.setToken(null)
    window.localStorage.removeItem('jwt')
    this.user = null
    history.push('/')
  }

  // get the user that matches the token
  getUser = async () => {
    try {
      const user = await agent.Account.current()
      // observable can only be modified inside an action
      runInAction(() => (this.user = user))
    } catch (error) {
      console.log(error)
    }
  }

  register = async (creds: UserFormValues) => {
    try {
      const user = await agent.Account.register(creds)
      // save the token in common store
      store.commonStore.setToken(user.token)
      // modify an observable inside an action
      runInAction(() => (this.user = user))
      history.push('/activities')
      store.modalStore.closeModal()
    } catch (error) {
      throw error
    }
  }

  // store photo url in "user.image"
  setImage = (image: string) => {
    if (this.user) this.user.image = image
  }

  // set the display name
  setDisplayName = (name: string) => {
    if (this.user) this.user.displayName = name
  }
}
