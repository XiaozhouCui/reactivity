import { Profile } from "./profile";

export interface Activity {
  id: string
  title: string
  date: Date | null
  description: string
  category: string
  city: string
  venue: string
  hostUsername: string
  isCancelled: boolean
  isGoing: boolean
  isHost: boolean
  host?: Profile
  attendees: Profile[]
}

export class Activity implements Activity {
  // populate all properties into activity
  constructor(init?: ActivityFormValues) {
    Object.assign(this, init)
  }
}

// use a class to auto convert the activity object from API using constructor
export class ActivityFormValues {
  // properties needed in the create activity form
  id?: string = undefined
  title: string = ''
  category: string = ''
  description: string = ''
  date: Date | null = null
  city: string = ''
  venue: string = ''

  constructor(activity?: ActivityFormValues) {
    if (activity) {
      // map to view model
      this.id = activity.id
      this.title = activity.title
      this.category = activity.category
      this.description = activity.description
      this.date = activity.date
      this.venue = activity.venue
      this.city = activity.city
    }
  }
}
