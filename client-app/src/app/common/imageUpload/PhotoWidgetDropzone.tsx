import React, { useCallback } from 'react'
import { useDropzone } from 'react-dropzone'
import { Header, Icon } from 'semantic-ui-react'

interface Props {
  setFiles: (files: any) => void
}

const PhotoWidgetDropzone = ({ setFiles }: Props) => {
  // style of inactive dropzone
  const dzStyles = {
    border: 'dashed 3px #eee',
    borderColor: '#eee',
    borderRadius: 5,
    paddingTop: 30,
    textAlign: 'center' as 'center',
    height: 200,
  }

  // style of active dropzone
  const dzActive = {
    ...dzStyles,
    borderColor: 'green',
  }

  const onDrop = useCallback((acceptedFiles) => {
    // Preview: store the image URL in file.preview
    setFiles(acceptedFiles.map((file: any) => Object.assign(file, {
      preview: URL.createObjectURL(file)
    })))
  }, [setFiles])

  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop })

  return (
    <div {...getRootProps()} style={isDragActive ? dzActive : dzStyles}>
      <input {...getInputProps()} />
      <Icon name='upload' size='huge' />
      <Header content='Drop image here' />
    </div>
  )
}

export default PhotoWidgetDropzone
