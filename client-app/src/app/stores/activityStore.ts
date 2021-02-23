import { makeAutoObservable } from 'mobx'

export default class ActivityStore {
  // MobX Observable: class properties
  title = 'Hello from MobX!'
  
  constructor() {
    // makeAutoObservable will use "this" class
    makeAutoObservable(this)
    // makeObservable(this, {
    //   title: observable,
    //   setTitle: action
    // })
  }

  // MobX Action: class methods
  // arrow function auto-bind setTitle() to "this" class
  setTitle = () => {
    this.title = this.title + '!'
  }
}
