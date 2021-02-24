import { Activity } from './../models/activity'
import { makeAutoObservable, runInAction } from 'mobx'
import agent from '../api/agent'
import { v4 as uuid } from 'uuid'
export default class ActivityStore {
  // MobX Observables: class properties
  // activities: Activity[] = []
  activityRegistry = new Map<string, Activity>() // { id1: activity1, id2: activity2, ... }
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
        // only keep the date part of iso-string
        activity.date = activity.date.split('T')[0]
        // mutating state is fine in MobX, as opposed to REDUX
        // this.activities.push(activity)
        this.activityRegistry.set(activity.id, activity)
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

  selectActivity = (id: string) => {
    // this.selectedActivity = this.activities.find((a) => a.id === id)
    this.selectedActivity = this.activityRegistry.get(id)
  }

  cancelSelectedActivity = () => {
    // arrow function auto-bind setTitle() to "this" class
    this.selectedActivity = undefined
  }

  openForm = (id?: string) => {
    id ? this.selectActivity(id) : this.cancelSelectedActivity()
    this.editMode = true
  }

  closeForm = () => {
    this.editMode = false
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
        // this.activities = [
        //   ...this.activities.filter((a) => a.id !== activity.id),
        //   activity,
        // ]
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
        // this.activities = [...this.activities.filter((a) => a.id !== id)]
        this.activityRegistry.delete(id)
        if (this.selectedActivity?.id === id) this.cancelSelectedActivity()
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
