import React from 'react'
import { observer } from 'mobx-react-lite'
import { Item, Segment } from 'semantic-ui-react'
import { useStore } from '../../../app/stores/store'
import ActivityListItem from './ActivityListItem'

const ActivityList = () => {
  const { activityStore } = useStore()
  const { activitiesByDate } = activityStore

  return (
    <Segment>
      <Item.Group divided>
        {activitiesByDate.map((activity) => (
          <ActivityListItem key={activity.id} activity={activity} />
        ))}
      </Item.Group>
    </Segment>
  )
}

export default observer(ActivityList)
