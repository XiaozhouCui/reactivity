import { Activity } from './../models/activity'
import { makeAutoObservable, runInAction } from 'mobx'
import agent from '../api/agent'
import { v4 as uuid } from 'uuid'
export default class ActivityStore {
  // MobX Observables: class properties
  activityRegistry = new Map<string, Activity>() // initialise Map object: { id1: activity1, id2: activity2, ... }
  selectedActivity: Activity | undefined = undefined
  editMode = false
  loading = false
  loadingInitial = true

  constructor() {
    // makeAutoObservable will auto convert class properties into MobX Observables, and methods into MobX Actions
    makeAutoObservable(this)
    // makeObservable(this, {
    //   title: observable,
    //   setTitle: action
    // })
  }

  // computed property as getter
  get activitiesByDate() {
    // sort the activities by date as an array
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => Date.parse(a.date) - Date.parse(b.date)
    )
  }

  // MobX Actions: class methods
  loadActivities = async () => {
    try {
      const activities = await agent.Activities.list()
      activities.forEach((activity) => {
        // arrow function auto-bind to "this" class
        this.setActivity(activity)
      })
      this.setLoadingInitial(false)
    } catch (error) {
      console.log(error)
      this.setLoadingInitial(false)
    }
  }

  // only MobX Action can change state
  loadActivity = async (id: string) => {
    let activity = this.getActivity(id)
    if (activity) {
      // if activity is already in MobX store, no need to load from API
      this.selectedActivity = activity
    } else {
      this.loadingInitial = true
      try {
        activity = await agent.Activities.details(id)
        this.setActivity(activity)
        this.setLoadingInitial(false)
      } catch (error) {
        console.log(error)
        this.setLoadingInitial(false)
      }
    }
  }

  // private helper function
  private setActivity = (activity: Activity) => {
    // only keep the date part of iso-string
    activity.date = activity.date.split('T')[0]
    // mutating state is fine in MobX, as opposed to REDUX
    // this.activities.push(activity)
    this.activityRegistry.set(activity.id, activity)
  }

  // private helper function
  private getActivity = (id: string) => {
    return this.activityRegistry.get(id)
  }

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state
  }

  createActivity = async (activity: Activity) => {
    this.loading = true
    activity.id = uuid()
    try {
      await agent.Activities.create(activity)
      // any state-changing steps after "await" need to be wrapped in action
      runInAction(() => {
        // this.activities.push(activity)
        this.activityRegistry.set(activity.id, activity)
        this.selectedActivity = activity
        this.editMode = false
        this.loading = false
      })
    } catch (error) {
      console.log(error)
      runInAction(() => {
        this.loading = false
      })
    }
  }

  updateActivity = async (activity: Activity) => {
    this.loading = true
    try {
      await agent.Activities.update(activity)
      runInAction(() => {
        this.activityRegistry.set(activity.id, activity)
        this.selectedActivity = activity
        this.editMode = false
        this.loading = false
      })
    } catch (error) {
      console.log(error)
      runInAction(() => {
        this.loading = false
      })
    }
  }

  deleteActivity = async (id: string) => {
    this.loading = true
    try {
      await agent.Activities.delete(id)
      runInAction(() => {
        this.activityRegistry.delete(id)
        this.loading = false
      })
    } catch (error) {
      console.log(error)
      runInAction(() => {
        this.loading = false
      })
    }
  }
}
