import React, { useEffect, useState } from 'react'
import { Button, Grid, Header, Image } from 'semantic-ui-react'
import PhotoWidgetCropper from './PhotoWidgetCropper'
import PhotoWidgetDropzone from './PhotoWidgetDropzone'

const PhotoUploadWidget = () => {
  const [files, setFiles] = useState<any>([])
  const [cropper, setCropper] = useState<Cropper>()

  function onCrop() {
    // check if there is cropper set
    if (cropper) {
      cropper.getCroppedCanvas().toBlob((blob) => console.log(blob))
    }
  }

  // clean up preview blob in memory
  useEffect(() => {
    return () => {
      files.forEach((file: any) => URL.revokeObjectURL(file.preview))
    }
  }, [files])

  return (
    <Grid>
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 1 - Add Photo' />
        <PhotoWidgetDropzone setFiles={setFiles} />
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 2 - Resize ihoto' />
        {files && files.length > 0 && (
          <PhotoWidgetCropper
            setCropper={setCropper}
            imagePreview={files[0].preview}
          />
        )}
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='Step 3 - Preview & Upload' />
        {files && files.length > 0 && (
          <>
            <div
              className='img-preview'
              style={{ minHeight: 200, overflow: 'hidden' }}
            />
            <Button.Group widths={2}>
              <Button onClick={onCrop} positive icon='check' />
              <Button onClick={() => setFiles([])} icon='close' />
            </Button.Group>
          </>
        )}
      </Grid.Column>
    </Grid>
  )
}

export default PhotoUploadWidget
