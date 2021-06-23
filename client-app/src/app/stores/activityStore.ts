import { Pagination } from '../models/pagination';
import { Activity, ActivityFormValues } from './../models/activity'
import { makeAutoObservable, runInAction } from 'mobx'
import agent from '../api/agent'
import { format } from 'date-fns'
import { store } from './store'
import { Profile } from '../models/profile'

export default class ActivityStore {
  // MobX Observables: class properties
  activityRegistry = new Map<string, Activity>() // initialise Map object: { id1: activity1, id2: activity2, ... }
  selectedActivity: Activity | undefined = undefined
  editMode = false
  loading = false
  loadingInitial = false
  pagination: Pagination | null = null

  constructor() {
    // makeAutoObservable will auto convert class properties into MobX Observables, and methods into MobX Actions
    makeAutoObservable(this)
    // makeObservable(this, {
    //   title: observable,
    //   setTitle: action
    // })
  }

  // computed function as getter
  get activitiesByDate() {
    // sort the activities by date as an array
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => a.date!.getTime() - b.date!.getTime()
    )
  }

  // another computed getter function
  get groupedActivities() {
    // return an array of [date, activities] arrays
    return Object.entries(
      // activities grouped by date
      this.activitiesByDate.reduce((activities, activity) => {
        // get the date string with date-fns
        const date = format(activity.date!, 'dd MMM yyyy')
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity]
        return activities
      }, {} as { [key: string]: Activity[] }) // {} is the initial value
    )
  }

  // MobX Actions: class methods
  loadActivities = async () => {
    this.loadingInitial = true
    try {
      const result = await agent.Activities.list()
      result.data.forEach((activity) => {
        // arrow function auto-bind to "this" class
        this.setActivity(activity)
      })
      this.setPagination(result.pagination)
      this.setLoadingInitial(false)
    } catch (error) {
      console.log(error)
      this.setLoadingInitial(false)
    }
  }

  // helper method to set pagination when loading activities
  setPagination = (pagination: Pagination) => {
    this.pagination = pagination
  }

  // only MobX Action can change state
  loadActivity = async (id: string) => {
    let activity = this.getActivity(id)
    if (activity) {
      // if activity is already in MobX store, no need to load from API
      this.selectedActivity = activity
      return activity
    } else {
      this.loadingInitial = true
      try {
        activity = await agent.Activities.details(id)
        this.setActivity(activity)
        runInAction(() => {
          this.selectedActivity = activity
        })
        this.setLoadingInitial(false)
        return activity
      } catch (error) {
        console.log(error)
        this.setLoadingInitial(false)
      }
    }
  }

  // private helper function
  private setActivity = (activity: Activity) => {
    const user = store.userStore.user
    // populate user properties in activity
    if (user) {
      // if the logged in user is in attendee's list, set isGoing to true
      activity.isGoing = activity.attendees!.some(
        (a) => a.username === user.username
      )
      activity.isHost = activity.hostUsername === user.username
      activity.host = activity.attendees?.find(
        (x) => x.username === activity.hostUsername
      )
    }

    activity.date = new Date(activity.date!)
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

  createActivity = async (activity: ActivityFormValues) => {
    const user = store.userStore.user
    const attendee = new Profile(user!)
    try {
      await agent.Activities.create(activity)
      // add host user when creating activity
      const newActivity = new Activity(activity)
      newActivity.hostUsername = user!.username
      newActivity.attendees = [attendee]
      // update activityRegistry
      this.setActivity(newActivity)
      // any state-changing steps after "await" need to be wrapped in action
      runInAction(() => {
        this.selectedActivity = newActivity
      })
    } catch (error) {
      console.log(error)
    }
  }

  updateActivity = async (activity: ActivityFormValues) => {
    try {
      await agent.Activities.update(activity)
      runInAction(() => {
        if (activity.id) {
          let updatedActivity = {
            ...this.getActivity(activity.id),
            ...activity,
          }
          this.activityRegistry.set(activity.id, updatedActivity as Activity)
          this.selectedActivity = updatedActivity as Activity
        }
      })
    } catch (error) {
      console.log(error)
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

  // toggle attencance status
  updateAttendance = async () => {
    const user = store.userStore.user
    this.loading = true
    try {
      await agent.Activities.attend(this.selectedActivity!.id)
      runInAction(() => {
        if (this.selectedActivity?.isGoing) {
          // if user is quitting, then remove user from attendees list
          this.selectedActivity.attendees =
            this.selectedActivity.attendees?.filter(
              (a) => a.username !== user?.username
            )
          this.selectedActivity.isGoing = false
        } else {
          // if a user is signning up, create a new profile for this user
          // Instanciating Profile class will convert the object interface from User to Profile
          const attendee = new Profile(user!)
          this.selectedActivity?.attendees?.push(attendee)
          this.selectedActivity!.isGoing = true
        }
        // update activityRegistry
        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        )
      })
    } catch (error) {
      console.log(error)
    } finally {
      // FINALLY block: always turn off the loading flag no matter what happens
      runInAction(() => (this.loading = false))
    }
  }

  // host user toggle cancel activity
  cancelActivityToggle = async () => {
    this.loading = true
    try {
      await agent.Activities.attend(this.selectedActivity!.id)
      runInAction(() => {
        this.selectedActivity!.isCancelled = !this.selectedActivity?.isCancelled
        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        )
      })
    } catch (error) {
      console.log(error)
    } finally {
      runInAction(() => (this.loading = false))
    }
  }

  clearSelectedActivity = () => {
    this.selectedActivity = undefined
  }

  updateAttendeeFollowing = (username: string) => {
    // loop over all attendees in each activity
    this.activityRegistry.forEach((activity) => {
      activity.attendees.forEach((attendee) => {
        if (attendee.username === username) {
          // if already following, decrease followers count by one, otherwise increase by one
          attendee.following
            ? attendee.followersCount--
            : attendee.followersCount++
          // toggle following status
          attendee.following = !attendee.following
        }
      })
    })
  }
}
