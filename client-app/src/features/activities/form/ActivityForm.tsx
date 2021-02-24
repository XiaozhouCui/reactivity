import { observer } from 'mobx-react-lite'
import React, { ChangeEvent, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { Button, Form, Segment } from 'semantic-ui-react'
import LoadingComponent from '../../../app/layout/LoadingComponent'
import { useStore } from '../../../app/stores/store'

const ActivityForm = () => {
  const { activityStore } = useStore()
  const { createActivity, updateActivity, loading, loadActivity, loadingInitial } = activityStore
  const { id } = useParams<{id: string}>()

  const [activity, setActivity] = useState({
    id: '',
    title: '',
    category: '',
    description: '',
    date: '',
    city: '',
    venue: '',
  })

  useEffect(() => {
    // "!" will override ts check
    if (id) loadActivity(id).then(activity => setActivity(activity!))
  }, [id, loadActivity])

  const handleSubmit = () => {
    activity.id ? updateActivity(activity) : createActivity(activity)
  }

  const handleInputChange = (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const {name, value} = event.target; // get attributes
    setActivity({ ...activity, [name]: value })
  }

  if (loadingInitial) return <LoadingComponent content='Loading activity...' />

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete='off'>
        <Form.Input placeholder='Title' value={activity.title} name='title' onChange={handleInputChange} />
        <Form.TextArea placeholder='Description' value={activity.description} name='description' onChange={handleInputChange} />
        <Form.Input placeholder='Category' value={activity.category} name='category' onChange={handleInputChange} />
        <Form.Input type='date' placeholder='Date' value={activity.date} name='date' onChange={handleInputChange} />
        <Form.Input placeholder='City' value={activity.city} name='city' onChange={handleInputChange} />
        <Form.Input placeholder='Venue' value={activity.venue} name='venue' onChange={handleInputChange} />
        <Button loading={loading} floated='right' positive type='submit' content='Submit' />
        <Button floated='right' type='button' content='Cancel' />
      </Form>
    </Segment>
  )
}

export default observer(ActivityForm)
