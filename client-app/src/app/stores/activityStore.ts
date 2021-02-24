import { Activity } from './../models/activity'
import { makeAutoObservable } from 'mobx'
import agent from '../api/agent'

export default class ActivityStore {
  // MobX Observables: class properties
  activities: Activity[] = []
  selectedActivity: Activity | null = null
  editMode = false
  loading = false
  loadingInitial = false

  constructor() {
    // makeAutoObservable will auto convert class properties into MobX Observables, and methods into MobX Actions
    makeAutoObservable(this)
    // makeObservable(this, {
    //   title: observable,
    //   setTitle: action
    // })
  }

  // MobX Actions: class methods
  loadActivities = async () => {
    // arrow function auto-bind setTitle() to "this" class
    this.setLoadingInitial(true)
    try {
      const activities = await agent.Activities.list()
      activities.forEach((activity) => {
        // only keep the date part of iso-string
        activity.date = activity.date.split('T')[0]
        // mutating state is fine in MobX, as opposed to REDUX
        this.activities.push(activity)
      })
      this.setLoadingInitial(false)
    } catch (error) {
      console.log(error)
      this.setLoadingInitial(false)
    }
  }

  // only MobX Action can change state
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state
  }
}
