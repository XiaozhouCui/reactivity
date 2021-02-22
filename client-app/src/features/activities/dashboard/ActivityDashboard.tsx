import React from 'react'
import { Grid } from 'semantic-ui-react'
import { Activity } from '../../../app/models/activity'
import ActivityList from './ActivityList'

interface Props {
  activities: Activity[]
}

const ActivityDashboard = ({ activities }: Props) => {
  return (
    <Grid>
      <Grid.Column width="10">
        <ActivityList activities={activities} />
      </Grid.Column>
    </Grid>
  )
}

export default ActivityDashboard
