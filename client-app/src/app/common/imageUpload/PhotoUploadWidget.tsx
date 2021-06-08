import React from 'react'
import { Grid, Header } from 'semantic-ui-react'
import PhotoWidgetDropzone from './PhotoWidgetDropzone'

const PhotoUploadWidget = () => {
  return (
    <Grid>
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 1 - Add Photo' />
        <PhotoWidgetDropzone />
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 2 - Resize ihoto' />
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 3 - Preview & Upload' />
      </Grid.Column>
    </Grid>
  )
}

export default PhotoUploadWidget
