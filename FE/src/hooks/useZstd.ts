'use client'

import { useEffect, useState } from 'react'

// Hàm giải nén Zstd với dictionary
const decompressZstdWithDict = async (compressed: Uint8Array, dict: Uint8Array): Promise<Uint8Array> => {
    return new Promise((resolve, reject) => {
        // Tải module Zstd WASM
        (window as any).ZstdCodec.run((zstd: any) => {
            try {
                // Tạo instance
                const simple = new zstd.Simple()
                // Tạo dictionary
                const dictionary = new zstd.Dictionary(dict)
                // Giải nén
                const decompressed = simple.decompress(compressed, dictionary)
                resolve(decompressed)
            } catch (e) {
                reject(e)
            }
        })
    })
}

export const ZstdDemo = (compressedData: Uint8Array, dictionaryData: Uint8Array): string => {
    const [output, setOutput] = useState<string>('')

    useEffect(() => {
        // Giả lập dữ liệu nén và dictionary
        // Thực tế bạn cần lấy chúng từ API, file, hoặc props
        decompressZstdWithDict(compressedData, dictionaryData)
            .then((result) => {
                // Giả sử dữ liệu gốc là chuỗi UTF-8
                setOutput(new TextDecoder().decode(result))
            })
            .catch((err) => setOutput('Error: ' + err))
    }, [])

    return output;
}